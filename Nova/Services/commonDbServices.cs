using System.Collections.Generic;
using Nova.Models;
using System.Data.SqlClient;
using System.Data;
using Nova.common;

namespace Nova.Services
{
    public static class commonDbServices
    {

        public static string DBConnection = "data source=.\\SQLExpress;initial catalog=DisCheckOut;integrated security=True;";

        /// <summary>
        /// Return the list of category
        /// </summary>
        /// <returns></returns>
        public static List<Category> GetCatgoryList()
        {
            List<Category> onlineShopCategory = new List<Category>();
            SqlConnection con = new SqlConnection(DBConnection);
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

        /// <summary>
        /// Get the sub category list based on department id
        /// </summary>
        /// <param name="catId">department id</param>
        /// <returns></returns>
        public static List<SubCatgory> GetSubCatgoryList(int catId)
        {
            List<SubCatgory> onlineShopSubCategory = new List<SubCatgory>();
            SqlConnection con = new SqlConnection(DBConnection);
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
    }
}