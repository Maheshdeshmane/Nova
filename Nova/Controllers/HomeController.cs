﻿using System.Collections.Generic;
using System.Web.Mvc;
using Nova.Models;
using Nova.Services;
using System.Data.SqlClient;
using System.Data;
using Nova.common;
using System.Linq;

namespace Nova.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            BaseRequest _request = new BaseRequest();
            List<SelectListItem> catNames = new List<SelectListItem>();
            List<Category> catList = GetCatgoryList();
            catList.ForEach(x => catNames.Add(new SelectListItem { Text = x.CatgorieName, Value = x.CatgorieId.ToString() }));
            _request.category = catNames;
            _request.subCategory = new List<SelectListItem>();
            return View(_request);
        }




        public List<Category> GetCatgoryList()
        {
            List<Category> onlineShopCategory = new List<Category>();
            SqlConnection con = new SqlConnection("data source=.\\SQLExpress;initial catalog=DisCheckOut;integrated security=True;");
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

        public List<SubCatgory> GetSubCatgoryList(int catId)
        {
            List<SubCatgory> onlineShopSubCategory = new List<SubCatgory>();
            SqlConnection con = new SqlConnection("data source=.\\SQLExpress;initial catalog=DisCheckOut;integrated security=True;");
            var cmd = new SqlCommand("dbo.[getSubCatgoryList]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@catId", SqlDbType.Int)).Value = catId;//Pass the parameter for lookingAtLat
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

        //Action result for ajax call
        [HttpPost]
        public ActionResult GetSubCatgoryByCategoryId(int stateid)
        {
            List<SubCatgory> objCat = new List<SubCatgory>();
            List<SubCatgory> subCatgory = GetSubCatgoryList(stateid);
            objCat = subCatgory.Where(m => m.CatgorieId == stateid).ToList();
            SelectList obgcity = new SelectList(objCat, "SubCatgorieId", "SubCatgorieName", 0);
            return Json(obgcity);
        }


    }
}