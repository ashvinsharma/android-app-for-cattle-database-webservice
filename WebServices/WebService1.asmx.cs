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
        [WebMethod]
        public void Authenticate(string LoginUserId, string Pwd) {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            con.Open();
            using(SqlCommand cmd = new SqlCommand("SP_MobileAuthenticateLogin", con)) {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@loginname", LoginUserId);
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // Create an instance of DataSet.
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

                //Authentication Process--
                string usernameDB = (string)rows[0]["LoginUser_Name"];
                string hashKeyDB = (string)rows[0]["UserPassword"];
                if(usernameDB.Equals(LoginUserId)) {
                    if(Pwd.Equals(hashKeyDB)) {
                        this.Context.Response.ContentType = "application/json; charset=utf-8";
                        this.Context.Response.Write(serializer.Serialize(new { rows }));

                    } else {
                        this.Context.Response.Write("Authentication Failed!: Wrong Password!");
                    }
                } else {
                    this.Context.Response.Write("Authentication Failed!: Wrong Username!");
                }

            }
        }
        [WebMethod]
        public void CompleteSpinnersTownVillage(int area, int stateID, int districtID, int blockID, int centreID) {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            con.Open();

            string procedure = null;
            //area = 1 is rural
            if(area == 1) {
                procedure = "SP_FetchVillege";
            } else if(area == 2) { //area = 2 is urban
                procedure = "SP_FetchTown";
            }
            using(SqlCommand cmd = new SqlCommand(procedure, con)){
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@State_ID", stateID);
                cmd.Parameters.AddWithValue("@District_ID", districtID);
                cmd.Parameters.AddWithValue("@Block_ID", blockID);
                cmd.Parameters.AddWithValue("@Centre_ID", centreID);
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
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
