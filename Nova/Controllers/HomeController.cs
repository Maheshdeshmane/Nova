using System.Collections.Generic;
using System.Web.Mvc;
using Nova.Models;
using Nova.Models.OnlineShopModels;
using Nova.Services;
using System.Data.SqlClient;
using System.Data;
using Nova.common;
using System.Linq;
using System.Text;
using System;
using System.Configuration;

namespace Nova.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            BaseRequest _request = new BaseRequest();
            List<SelectListItem> catNames = new List<SelectListItem>();
            ConnectionString = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ToString();
            List<Category> catList = GetCatgoryList();
            List<SubCatgory> objCat = new List<SubCatgory>();
            List<SubCatgory> subCatgory = GetSubCatgoryList();        
            int searchTill = 20;
            bool isSessionRequired = Session.IsNewSession;
            if (!isSessionRequired)
            {
                isSessionRequired=string.IsNullOrEmpty(Session["IsNewSession"].ToString())?false:(bool)Session["IsNewSession"];
            }

            if (Session.IsNewSession)
            {
                Session["CategoryList"] = catList;
                Session["SubCategoryList"] = subCatgory;
                Session["SelectedCategory"] = string.Empty;
                Session["SelectedSubCategory"] = string.Empty;
                Session["SearchTillKm"] = 20;
                Session["SortBy"] = "Discount";
                Session["Keyword"] = string.Empty;
                Session["Locality"] = string.Empty;
                Session["LocalityLat"] = string.Empty;
                Session["LocalityLong"] = string.Empty;
                Session["refresh"] = "NewPage";
            }
            else {
                Int32.TryParse(Session["SearchTillKm"].ToString(), out searchTill);
                _request.lookingAt = Session["Locality"] as string;
                _request.lookingTillDistance = searchTill;
                _request.lookingBy = Session["SortBy"] as string;
                _request.lookingFor= Session["Keyword"] as string;
                _request.lookingAt= Session["Locality"] as string;
                _request.lookingAtLat = Session["LocalityLat"] as string;
                _request.lookingAtLng = Session["LocalityLong"] as string;
                _request.selectedCategory = Session["SelectedCategory"] as string;
                Session["refresh"] = "OldPage";

                if (isSessionRequired)
                    Session["discountsList"] = null;
            }
            Session["IsNewSession"] = false;
            catList.ForEach(x => catNames.Add(new SelectListItem { Text = x.CatgorieName, Value = x.CatgorieId.ToString() }));
            _request.category = catNames;
            _request.subCategory = new List<SelectListItem>();
            return View(_request);
        }

        [Route("Shop/{ShopName}")]
        public ActionResult Shop(string ShopName)
        {

            OnlineShop onlineShop = new OnlineShop();
            onlineShop.Home = GetOnlineShopHomeDetails(ShopName);

            if (onlineShop.Home == null)
            {
                ///TODO- Return to homepage
            }
            onlineShop.Products = GetOnlineShopProductDetails(ShopName);
            onlineShop.TeamMembers = GetOnlineShopTeamMemberDetails(ShopName);
            onlineShop.Discounts = GetOnlineShopDiscountDetails(ShopName);

            if (onlineShop.Products != null)
                onlineShop.ShopCategory = onlineShop.Products.Select(product => product.FolderName).Distinct().ToList();

            onlineShop.Awards = GetOnlineShopHomeAwards(ShopName);
            onlineShop.Testimonials = GetOnlineShopTestimonials(ShopName);
            return View(onlineShop);
            
            
        }

        [HttpPost]
        public ActionResult home()
        {
            Session["IsNewSession"] = true;
            return Json("");
        }

        public string ConnectionString { get; set; } = "data source=.\\SQLExpress;initial catalog=DisCheckOut;integrated security=True;";
        //<!--connectionString="data source=182.50.133.111;initial catalog=whopp4km_local;User ID=mahesh_admin;Password=dEshmane@123@db;"-->
        //Action result for ajax call
        [HttpPost]
        public ActionResult GetSubCatgoryByCategoryId(int stateid)
        {
            List<SubCatgory> objCat = new List<SubCatgory>();
            List<SubCatgory> subCatgory = Session["SubCategoryList"] as List<SubCatgory>;
            objCat = subCatgory.Where(m => m.CatgorieId == stateid).ToList();

            Session["SelectedSubCategoryList"] = objCat;
            SelectList obgcity = new SelectList(objCat, "SubCatgorieId", "SubCatgorieName", 0);
            return Json(obgcity);
        }

        [HttpPost]
        public ActionResult Search(int categrory, int subCategory, string lookingAt, string lookingAtLat, string lookingAtLng, string lookingFor, string lookingBy, string lookingTillDistance)
        {

            if (!string.IsNullOrEmpty(lookingAt))
            {
                lookingAt = RemoveWildCardChar(lookingAt);
            }

            if (!string.IsNullOrEmpty(lookingFor))
            {
                lookingFor = RemoveWildCardChar(lookingFor);
            }

            if (!string.IsNullOrEmpty(lookingTillDistance))
            {
                lookingTillDistance = RemoveWildCardChar(lookingTillDistance);
            }
            
            Session["SelectedCategory"] = categrory.ToString();
            Session["SelectedSubCategory"] = subCategory.ToString();
            Session["SearchTillKm"] = lookingTillDistance;
            Session["SortBy"] = lookingBy;
            Session["Keyword"] = lookingFor;
            Session["Locality"] = lookingAt;
            Session["LocalityLat"] = lookingAtLat;
            Session["LocalityLong"] = lookingAtLng;


            if (string.IsNullOrEmpty(lookingAtLat))
            {
                return Json(getDiscountHtml(GetDiscountDetailsWithoutLocation(categrory, subCategory, lookingFor)));
            }
            else
            {
                return Json(getDiscountHtml(GetDiscountDetailsWithLocation(categrory, subCategory, lookingFor, lookingAtLat, lookingAtLng, lookingTillDistance, lookingBy)));
            }     
        }
               

        [HttpPost]
        public ActionResult ExoplereSearch(int catId, int subCatId, string searchText)
        {
           return Json(getDiscountHtml(GetDiscountDetailsWithoutLocation(catId,subCatId,searchText)));
        }


        #region "DB Methods"
        public List<Category> GetCatgoryList()
        {            
            List<Category> onlineShopCategory = new List<Category>();

            if (string.IsNullOrEmpty(ConnectionString))
                ConnectionString = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ToString();

            SqlConnection con = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand("dbo.[getCatgoryList]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                onlineShopCategory = new ReflectionPopulator<Category>().CreateList(cmd.ExecuteReader());

            }
            finally
            {
                if (con.State != ConnectionState.Closed)
                    con.Close();
            }
            return onlineShopCategory;
        }

        public List<SubCatgory> GetSubCatgoryList()
        {
            List<SubCatgory> onlineShopSubCategory = new List<SubCatgory>();

            if (string.IsNullOrEmpty(ConnectionString))
                ConnectionString = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ToString();

            SqlConnection con = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand("dbo.[getSubCatgoryList]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                onlineShopSubCategory = new ReflectionPopulator<SubCatgory>().CreateList(cmd.ExecuteReader());

            }
            finally
            {
                if (con.State != ConnectionState.Closed)
                    con.Close();
            }
            return onlineShopSubCategory;
        }
        public List<DiscountDetailsModel> GetDiscountDetailsWithoutLocation(int catId, int subCatId, string searchText)
        {

            List<DiscountDetailsModel> discountDetails = new List<DiscountDetailsModel>();

            if (string.IsNullOrEmpty(ConnectionString))
                ConnectionString = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ToString();

            SqlConnection con = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand("dbo.[getDiscountCheckWithoutLocation]", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@DepartmentId", SqlDbType.Int)).Value = catId;//Pass the parameter for lookingAtLat }


            cmd.Parameters.Add(new SqlParameter("@SubDepartmentId", SqlDbType.Int)).Value = subCatId;//Pass the parameter for lookingAtLat }




            cmd.Parameters.Add(new SqlParameter("@SEARCHTEXT", SqlDbType.VarChar)).Value = string.IsNullOrEmpty(searchText) ? string.Empty : searchText;//Pass the parameter for lookingAtLat }


            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                discountDetails = new ReflectionPopulator<DiscountDetailsModel>().CreateList(cmd.ExecuteReader());

            }
            finally
            {
                if (con.State != ConnectionState.Closed)
                    con.Close();
            }

            return discountDetails;

        }
        public List<DiscountDetailsModel> GetDiscountDetailsWithLocation(int catId, int subCatId, string searchText=null,  string locationLat = null, string locationLng = null, string seacrhTill = null, string sortBy = null)
        {

            List<DiscountDetailsModel> discountDetails = new List<DiscountDetailsModel>();
            if(string.IsNullOrEmpty(ConnectionString))
                ConnectionString = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ToString();

            SqlConnection con = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand("[dbo].[getDiscountCheckForLocation]", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@DepartmentId", SqlDbType.Int)).Value = catId;//Pass the parameter for lookingAtLat }
            cmd.Parameters.Add(new SqlParameter("@SubDepartmentId", SqlDbType.Int)).Value = subCatId;//Pass the parameter for lookingAtLat }
            cmd.Parameters.Add(new SqlParameter("@SEARCHTEXT", SqlDbType.VarChar)).Value = string.IsNullOrEmpty(searchText) ? string.Empty : searchText;//Pass the parameter for lookingAtLat }
            cmd.Parameters.Add(new SqlParameter("@SEARCH_DISTANCE", SqlDbType.VarChar)).Value = string.IsNullOrEmpty(seacrhTill) ? string.Empty : seacrhTill;
            cmd.Parameters.Add(new SqlParameter("@LAT", SqlDbType.VarChar)).Value = string.IsNullOrEmpty(locationLat) ? string.Empty : locationLat;
            cmd.Parameters.Add(new SqlParameter("@LNG", SqlDbType.VarChar)).Value = string.IsNullOrEmpty(locationLng) ? string.Empty : locationLng;
            cmd.Parameters.Add(new SqlParameter("@SORTORDEDR", SqlDbType.VarChar)).Value = string.IsNullOrEmpty(sortBy) ? string.Empty : sortBy;

            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                discountDetails = new ReflectionPopulator<DiscountDetailsModel>().CreateList(cmd.ExecuteReader());

            }
            finally
            {
                if (con.State != ConnectionState.Closed)
                    con.Close();
            }

            return discountDetails;

        }
        /// <summary>
        /// Get Shop Details by ShopName
        /// </summary>
        /// <param name="shopName">name of shop</param>
        /// <returns></returns>
        public OnlineShopHome GetOnlineShopHomeDetails(string shopName)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                ConnectionString = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ToString();

            OnlineShopHome onlineShopHome = new OnlineShopHome();
            SqlConnection con = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand("dbo.[getOnlineShopDetailsByName]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@SEARCHTEXT", SqlDbType.VarChar)).Value = shopName;//Pass the parameter for lookingAtLat
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    onlineShopHome.BusinessContact = (string)reader["BusinessContact"];
                    onlineShopHome.BusinessName = (string)reader["BusinessName"];
                    onlineShopHome.ComplaintContact = (string)reader["ComplaintContact"];
                    onlineShopHome.Email = (string)reader["Email"];
                    onlineShopHome.FacebookLink = (string)reader["FacebookLink"];
                    onlineShopHome.GoogleMapAddress = (string)reader["GoogleMapAddress"];
                    onlineShopHome.HeaderImage = (string)reader["HeaderImage"];
                    onlineShopHome.HeaderImage2 = (string)reader["HeaderImage2"];
                    onlineShopHome.HeaderImage3 = (string)reader["HeaderImage3"];
                    onlineShopHome.latitude = reader["latitude"].ToString();
                    onlineShopHome.LinkdInLink = (string)reader["LinkdInLink"];
                    onlineShopHome.longitude = reader["longitude"].ToString();
                    onlineShopHome.shopAddress = (string)reader["shopAddress"];
                    onlineShopHome.ShopName = (string)reader["ShopName"];
                    onlineShopHome.ShopType = (int)reader["ShopType"];
                    onlineShopHome.TwitterLink = (string)reader["TwitterLink"];
                    onlineShopHome.WebBusinessName = (string)reader["WebBusinessName"];
                    onlineShopHome.ShopAboutUs = reader["ShopAboutUs"]!=System.DBNull.Value? (string)reader["ShopAboutUs"]:string.Empty;
                    onlineShopHome.ShopValue1Tittle = reader["ShopValue1Tittle"] != System.DBNull.Value ? (string)reader["ShopValue1Tittle"] : string.Empty;
                    onlineShopHome.ShopValue1Desc = reader["ShopValue1Desc"] != System.DBNull.Value ? (string)reader["ShopValue1Desc"] : string.Empty;
                    onlineShopHome.ShopValue2Tittle = reader["ShopValue2Tittle"] != System.DBNull.Value ? (string)reader["ShopValue2Tittle"] : string.Empty;
                    onlineShopHome.ShopValue2Desc = reader["ShopValue2Desc"] != System.DBNull.Value ? (string)reader["ShopValue2Desc"] : string.Empty;
                    onlineShopHome.ShopValue3Tittle = reader["ShopValue3Tittle"] != System.DBNull.Value ? (string)reader["ShopValue3Tittle"] : string.Empty;
                    onlineShopHome.ShopValue3Desc = reader["ShopValue3Desc"] != System.DBNull.Value ? (string)reader["ShopValue3Desc"] : string.Empty;
                    onlineShopHome.ShopValue4Tittle = reader["ShopValue4Tittle"] != System.DBNull.Value ? (string)reader["ShopValue4Tittle"] : string.Empty;
                    onlineShopHome.ShopValue4Desc = reader["ShopValue4Desc"] != System.DBNull.Value ? (string)reader["ShopValue4Desc"] : string.Empty;
                    onlineShopHome.ShopOffersTagLine = reader["ShopOffersTagLine"] != System.DBNull.Value ? (string)reader["ShopOffersTagLine"] : string.Empty;
                    onlineShopHome.ShopTeamTagLine = reader["ShopTeamTagLine"] != System.DBNull.Value ? (string)reader["ShopTeamTagLine"] : string.Empty;
                    onlineShopHome.ShopTestimonialsTagLine = reader["ShopTestimonialsTagLine"] != System.DBNull.Value ? (string)reader["ShopTestimonialsTagLine"] : string.Empty;
                    onlineShopHome.ShopContactTagLine = reader["ShopContactTagLine"] != System.DBNull.Value ? (string)reader["ShopContactTagLine"] : string.Empty;
                    onlineShopHome.ShopContactDesc = reader["ShopContactDesc"] != System.DBNull.Value ? (string)reader["ShopContactDesc"] : string.Empty;
                    onlineShopHome.ShopTagSubLine = reader["ShopTagSubLine"] != System.DBNull.Value ? (string)reader["ShopTagSubLine"] : string.Empty;
                }
            }
            finally
            {
                if (con.State != ConnectionState.Closed)
                    con.Close();
            }
            return onlineShopHome;
        }

        /// <summary>
        /// Get Product Details by ShopName
        /// </summary>
        /// <param name="shopName">name of shop</param>
        /// <returns></returns>
        public List<OnlineShopProduct> GetOnlineShopProductDetails(string shopName)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                ConnectionString = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ToString();

            List<OnlineShopProduct> onlineShopProduct = new List<OnlineShopProduct>();
            SqlConnection con = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand("dbo.[getOnlineShopProductByName]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@SEARCHTEXT", SqlDbType.VarChar)).Value = shopName;//Pass the parameter for lookingAtLat
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                onlineShopProduct = new ReflectionPopulator<OnlineShopProduct>().CreateList(cmd.ExecuteReader());

            }
            finally
            {
                if (con.State != ConnectionState.Closed)
                    con.Close();
            }
            return onlineShopProduct;
        }

        /// <summary>
        /// Get Team Members Details by ShopName
        /// </summary>
        /// <param name="shopName">name of shop</param>
        /// <returns></returns>
        public List<OnlineShopTeamMember> GetOnlineShopTeamMemberDetails(string shopName)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                ConnectionString = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ToString();

            List<OnlineShopTeamMember> onlineShopTeamMembers = new List<OnlineShopTeamMember>();
            SqlConnection con = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand("dbo.[getOnlineShopTeamByName]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@SEARCHTEXT", SqlDbType.VarChar)).Value = shopName;//Pass the parameter for lookingAtLat
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                onlineShopTeamMembers = new ReflectionPopulator<OnlineShopTeamMember>().CreateList(cmd.ExecuteReader());

            }
            finally
            {
                if (con.State != ConnectionState.Closed)
                    con.Close();
            }
            return onlineShopTeamMembers;
        }

        /// <summary>
        /// Get Team Members Details by ShopName
        /// </summary>
        /// <param name="shopName">name of shop</param>
        /// <returns></returns>
        public List<OnlineShopDiscount> GetOnlineShopDiscountDetails(string shopName)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                ConnectionString = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ToString();

            List<OnlineShopDiscount> onlineShopDiscount = new List<OnlineShopDiscount>();
            SqlConnection con = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand("dbo.[getOnlineShopDiscountByName]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@SEARCHTEXT", SqlDbType.VarChar)).Value = shopName;//Pass the parameter for lookingAtLat
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                onlineShopDiscount = new ReflectionPopulator<OnlineShopDiscount>().CreateList(cmd.ExecuteReader());

            }
            finally
            {
                if (con.State != ConnectionState.Closed)
                    con.Close();
            }
            return onlineShopDiscount;
        }

        /// <summary>
        /// Get Shop Awards by ShopName
        /// </summary>
        /// <param name="shopName">name of shop</param>
        /// <returns></returns>
        public OnlineShopAwards GetOnlineShopHomeAwards(string shopName)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                ConnectionString = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ToString();

            OnlineShopAwards onlineShopAwards = new OnlineShopAwards();
            SqlConnection con = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand("dbo.[getOnlineShopHomeAwards]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@SEARCHTEXT", SqlDbType.VarChar)).Value = shopName;//Pass the parameter for lookingAtLat
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    onlineShopAwards.Awards = (int)reader["Awards"];
                    onlineShopAwards.FacebookLikes = (int)reader["FacebookLikes"];
                    onlineShopAwards.Followers = (int)reader["Followers"];
                    onlineShopAwards.HappyClients = (int)reader["HappyClients"];                    
                }
            }
            finally
            {
                if (con.State != ConnectionState.Closed)
                    con.Close();
            }
            return onlineShopAwards;
        }

        /// <summary>
        /// Get Team Members Details by ShopName
        /// </summary>
        /// <param name="shopName">name of shop</param>
        /// <returns></returns>
        public List<OnlineShopTestimonials> GetOnlineShopTestimonials(string shopName)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                ConnectionString = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ToString();

            List<OnlineShopTestimonials> onlineShopDiscount = new List<OnlineShopTestimonials>();
            SqlConnection con = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand("dbo.[getOnlineShopHomeTestimonials]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@SEARCHTEXT", SqlDbType.VarChar)).Value = shopName;//Pass the parameter for lookingAtLat
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                onlineShopDiscount = new ReflectionPopulator<OnlineShopTestimonials>().CreateList(cmd.ExecuteReader());

            }
            finally
            {
                if (con.State != ConnectionState.Closed)
                    con.Close();
            }
            return onlineShopDiscount;
        }

        #endregion

        #region common Methods
        public string getDiscountHtml(List<DiscountDetailsModel> data)
        {
            StringBuilder sb = new StringBuilder();
            int divRow = 0;
            sb.AppendLine("<div class=\"col-sm-9\">");

            sb.AppendLine("<div class=\"container\">");
            sb.AppendLine("<div class=\"row mb-5\">");

            if (data.Count == 0)
                sb.AppendLine("<div><br>Sorry, No Discount available for filter criteria! Try new search</div>");

            foreach (var discount in data)
            {

                if(divRow==0)
                    sb.AppendLine("<div class=\"row row-l-t\">");

                divRow++;
                sb.AppendLine("<div class=\"col-sm-6 col-md-4 col-lg-3 mt-4\">");
                sb.AppendLine("<div class=\"card card-inverse card-info\">");

                foreach (var img in discount.ShopOfferImages)
                {
                    var imageName = string.Format("https://www.w3schools.com/w3css/{0}", img);
                    sb.AppendLine(string.Format("<img class=\"card-img-top\" src='{0}'>", imageName));
                }


                sb.AppendLine("<div class=\"card-block\">");
                sb.AppendLine("<figure class=\"profile profile-inline\">");
                sb.AppendLine("<img src=\"http://success-at-work.com/wp-content/uploads/2015/04/free-stock-photos.gif\" class=\"profile-avatar\" alt=\"\">");
                sb.AppendLine("</figure>");
                sb.AppendLine(string.Format("<h4 class=\"card-title\">{0}</h4>", discount.ShopOffer));
                sb.AppendLine(string.Format("<div class=\"card-text\">{0}</div>", discount.ShopName));
                sb.AppendLine("</div>");

                sb.AppendLine("<div class=\"card-footer\">");
                sb.AppendLine(string.Format("<small>{0}</small>", discount.ShopAddress));
                sb.AppendLine(string.Format("<small>{0}</small>", discount.ShopContact));
                sb.AppendLine("<br>");

                sb.AppendLine(string.Format("<button class=\"btn btn-info controls pull-right btn-sm\" onClick=\"window.open('{0}'); \">Direction</button>",discount.ShopGoogleMapAddress));
                string onlineShop = string.Format("https://www.whopperbay.com/shop/{0}", discount.ShopOnlineAddress);
                sb.AppendLine(string.Format("<button class=\"btn btn-info controls pull-right btn-sm\" onClick=\"window.open('{0}'); \">Visit Shop</button>", onlineShop));
                sb.AppendLine("<br>");
                sb.AppendLine("</div>");

                sb.AppendLine("</div>");
                sb.AppendLine("</div>");

                if (divRow == 3)
                {
                    sb.AppendLine("</div>");
                    divRow = 0;
                }
            }

            if (divRow!=0 && divRow != 3)
                sb.AppendLine("</div>");

            sb.AppendLine("</div>");

            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            Session["discountsList"] = sb.ToString();
            return sb.ToString();
        }

        public string RemoveWildCardChar(string str)
        {
            var pattern = @"/\[]:|<>+=;'?*";
            return new string(str.Trim().Where(ch => !pattern.Contains(ch)).ToArray());
        }
        
        #endregion
    }
}