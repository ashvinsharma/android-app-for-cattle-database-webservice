using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebServices {
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService {

        [WebMethod]
        public void GetUserDetails() {

            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            con.Open();
            using(SqlCommand cmd = new SqlCommand("SP_fetechrecord", con)) {
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@LoginUser_ID", LoginUserId);
                //cmd.Parameters.AddWithValue("@LoginUser_Password", Pwd);
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // Create an instance of DataSet.-
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();

                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row = null;
                foreach(DataRow rs in dt.Rows) {
                    row = new Dictionary<string, object>();
                    foreach(DataColumn col in dt.Columns) {
                        row.Add(col.ColumnName, rs[col]);
                    }
                    rows.Add(row);
                }

                this.Context.Response.ContentType = "application/json; charset=utf-8";
                this.Context.Response.Write(serializer.Serialize(new { rows }));
            }

        }
        public void SendUserPass(string LoginUserId, string Pwd) {

            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            con.Open();
            using(SqlCommand cmd = new SqlCommand("SP_fetechrecord", con)) {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LoginUser_ID", LoginUserId);
                cmd.Parameters.AddWithValue("@LoginUser_Password", Pwd);
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // Create an instance of DataSet.-
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();

                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row = null;
                foreach(DataRow rs in dt.Rows) {
                    row = new Dictionary<string, object>();
                    foreach(DataColumn col in dt.Columns) {
                        row.Add(col.ColumnName, rs[col]);
                    }
                    rows.Add(row);
                }

                this.Context.Response.ContentType = "application/json; charset=utf-8";
                this.Context.Response.Write(serializer.Serialize(new { rows }));
            }
        }

    }
}
