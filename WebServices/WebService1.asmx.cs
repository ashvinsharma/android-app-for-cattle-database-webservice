using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
        public void CompleteSpinnerTownVillage(int area, int stateID, int districtID, int blockID, int centreID) {
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
            using(SqlCommand cmd = new SqlCommand(procedure, con)) {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@State_ID", stateID);
                cmd.Parameters.AddWithValue("@District_ID", districtID);
                cmd.Parameters.AddWithValue("@Block_ID", blockID);
                cmd.Parameters.AddWithValue("@Centre_ID", centreID);
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
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
        public void CompleteSpinnerBreed(int stateID, int species) {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            con.Open();

            using(SqlCommand cmd = new SqlCommand("SP_FetchBreeds", con)) {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@State_ID", stateID);
                cmd.Parameters.AddWithValue("@Species_ID", species);
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
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

                this.Context.Response.ContentType = "Application / json; charset=utf-8";
                this.Context.Response.Write(serializer.Serialize(new { rows }));
            }
        }

        [WebMethod]
        public void SendPrimaryRegistrationFormData(string area, int villageTownID, string ownerName, string fatherHusbandName, string category, string address, string mobileNumber, string telephoneNumber, int species, string earTagNumber, string animalName, string animalAge, int breed, string dateOfCalving, int lactationNumber, string sireDetails, string damDetails, string firstRecordingDate, string calfEarTag, int sex, int stateID, int districtID, int blockID, int centreID, string username) {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            con.Open();

            using(SqlCommand cmd = new SqlCommand("SP_AndroidInsertNewPrimaryRegistration", con)) {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EarTagNo", earTagNumber);
                cmd.Parameters.AddWithValue("@State_ID", stateID);
                cmd.Parameters.AddWithValue("@District_ID", districtID);
                cmd.Parameters.AddWithValue("@Block_ID", blockID);
                cmd.Parameters.AddWithValue("@Centre_ID", centreID);
                cmd.Parameters.AddWithValue("@Village_ID", villageTownID);
                cmd.Parameters.AddWithValue("@AnimalName", animalName);
                cmd.Parameters.AddWithValue("@OwnerName", ownerName);
                cmd.Parameters.AddWithValue("@FName", fatherHusbandName);
                cmd.Parameters.AddWithValue("@OCategory", category);
                cmd.Parameters.AddWithValue("@OAddress", address);
                cmd.Parameters.AddWithValue("@MobileNo", mobileNumber);
                cmd.Parameters.AddWithValue("@TelephoneNo", telephoneNumber);
                cmd.Parameters.AddWithValue("@AnimalAge", animalAge);
                cmd.Parameters.AddWithValue("@SireDetail", sireDetails);
                cmd.Parameters.AddWithValue("@DamNumber", damDetails);
                cmd.Parameters.AddWithValue("@Species_ID", species);
                cmd.Parameters.AddWithValue("@Breed_ID", breed);
                cmd.Parameters.AddWithValue("@Sex", sex);
                cmd.Parameters.AddWithValue("@EarTagNoCalf", calfEarTag);
                cmd.Parameters.AddWithValue("@DateOfCalving", dateOfCalving);
                cmd.Parameters.AddWithValue("@NumberofLactation", lactationNumber);
                cmd.Parameters.AddWithValue("@FirstRecDate", firstRecordingDate);
                cmd.Parameters.AddWithValue("@CreatedBy", username);
                cmd.Parameters.AddWithValue("@Area", area);

                int i = cmd.ExecuteNonQuery();
                this.Context.Response.ContentType = "Application / json; charset=utf-8";
                if(i == 1) {
                    this.Context.Response.Write(serializer.Serialize("Success"));
                }
            }
        }

        [WebMethod]
        public void GetPrimaryRegistrationList(string username) {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            con.Open();
            using(SqlCommand cmd = new SqlCommand("SP_FetchPrimayRegistration", con)) {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LoginUser_Name", username);

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

        [WebMethod]
        public void SendFirstMilkRecording(int pReg_ID, string earTagNo, int numberofLactation, string dateofFirstMilkRec, int dayofFirstMilkRec, float firstMilkRecMorning, float firstMilkRecNoon, float firstMilkRecEvening, int state_ID, int district_ID, int block_ID, int centre_ID, string createdBy, int typeofMilkRecorder, string nameMilkRecorder, string nameOfSupervisor) {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            con.Open();

            using(SqlCommand cmd = new SqlCommand("SP_InsertMilkRecording", con)) {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("PReg_ID", pReg_ID);
                cmd.Parameters.AddWithValue("EarTagNo", earTagNo);
                cmd.Parameters.AddWithValue("NumberofLactation", numberofLactation);
                cmd.Parameters.AddWithValue("DateofFirstMilkRec", dateofFirstMilkRec);
                cmd.Parameters.AddWithValue("DayofFirstMilkRec", dayofFirstMilkRec);
                cmd.Parameters.AddWithValue("FirstMilkRecMorning", firstMilkRecMorning);
                cmd.Parameters.AddWithValue("FirstMilkRecNoon", firstMilkRecNoon);
                cmd.Parameters.AddWithValue("FirstMilkRecEvening", firstMilkRecEvening);
                cmd.Parameters.AddWithValue("State_ID", state_ID);
                cmd.Parameters.AddWithValue("District_ID", district_ID);
                cmd.Parameters.AddWithValue("Block_ID", block_ID);
                cmd.Parameters.AddWithValue("Centre_ID", centre_ID);
                cmd.Parameters.AddWithValue("CreatedBy", createdBy);
                cmd.Parameters.AddWithValue("TypeofMilkRecorder", typeofMilkRecorder);
                cmd.Parameters.AddWithValue("NameMilkRecorder", nameMilkRecorder);
                cmd.Parameters.AddWithValue("NameOfSupervisor", nameOfSupervisor);

                int i = cmd.ExecuteNonQuery();
                this.Context.Response.ContentType = "Application / json; charset=utf-8";
                if(i == 1) {
                    this.Context.Response.Write(serializer.Serialize("Success"));
                }
            }
        }
    }
}