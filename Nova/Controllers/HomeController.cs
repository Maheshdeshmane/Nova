using System.Collections.Generic;
using System.Web.Mvc;
using Nova.Models;
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
            }
            catList.ForEach(x => catNames.Add(new SelectListItem { Text = x.CatgorieName, Value = x.CatgorieId.ToString() }));
            _request.category = catNames;
            _request.subCategory = new List<SelectListItem>();
            return View(_request);
        }

        public string ConnectionString { get; set; }
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
                sb.AppendLine("<div>Sorry No Records Found</div>");

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

                sb.AppendLine("<button class=\"btn btn-info controls pull-right btn-sm\" onClick=\"window.open('http://www.google.com'); \">Direction</button>");
                sb.AppendLine("<button class=\"btn btn-info controls pull-right btn-sm\" onClick=\"window.open('http://www.google.com'); \">Visit Shop</button>");
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