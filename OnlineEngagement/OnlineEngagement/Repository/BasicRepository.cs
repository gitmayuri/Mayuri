using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineEngagement.Models;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Text;

namespace OnlineEngagement.Repository
{
    public class BasicRepository
    {
        string strcon = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        internal List<UserDetail> EngagementAdminLogin(AdminModel lg)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    List<UserDetail> list = new List<UserDetail>();
                    SqlCommand cmd = new SqlCommand("SP_Login", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "SCHEDULING_ADMIN");
                    cmd.Parameters.AddWithValue("@EmailId", lg.Username);
                    cmd.Parameters.AddWithValue("@Password", lg.Password);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt != null)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (lg.Username == dt.Rows[i]["email"].ToString() && lg.Password == dt.Rows[i]["password"].ToString())
                            {
                                // List<AdminDetail> list = new List<AdminDetail>();
                                UserDetail li = new UserDetail();
                                li.Id = Convert.ToInt32(dt.Rows[i]["id"]);
                                li.Name = dt.Rows[i]["name"].ToString();
                                li.UserType = dt.Rows[i]["type"].ToString();
                                // li.UserTypeId = Convert.ToInt32(dt.Rows[i]["UserTypeId"]);
                                li.EmailId = dt.Rows[i]["email"].ToString();
                                //li.Contact = dt.Rows[i]["ContactNo"].ToString();
                                li.Password = dt.Rows[i]["password"].ToString();
                                li.Status = dt.Rows[i]["status"].ToString();

                                list.Add(li);
                                return list;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

        public bool PostAuthCode(authCode AC)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    con.Open();
                    SqlTransaction transaction;
                    // Start a local transaction.
                    
                    transaction = con.BeginTransaction("AuthCodeTransaction");
                    string str = "insert into tblPromotor (name,count,date,createdBy,comment) values ('" + AC.CreatedFor + "','" + AC.NoOfCodes + "','" + DateTime.Now + "','" + AC.userId + "','" + AC.Comment + "')";
                    SqlCommand cmd = new SqlCommand(str,con);
                   
                    cmd.Transaction = transaction;
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        //find last inserted value 
                        str = "select SCOPE_IDENTITY()";
                        cmd.CommandText = str;
                        int lastId = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                        if (lastId > 0)
                        {
                            //create a new datatable
                            DataTable acode = new DataTable();
                            acode.Columns.Add("prodid", typeof(int));
                            acode.Columns.Add("authcode", typeof(string));
                            acode.Columns.Add("date", typeof(DateTime));
                            acode.Columns.Add("status", typeof(string));
                            acode.Columns.Add("promotorId", typeof(int));
                            acode.Columns.Add("validity", typeof(int));

                            int validity = 0;
                            if (AC.ValidityInDays == "")
                                validity = 0;
                            else
                                validity = Convert.ToInt32(AC.ValidityInDays);
                            //create a new authcode and added to datatable
                            for (int j = 0; j < Convert.ToInt32(AC.NoOfCodes); j++)
                            {
                                string authcode = GenerateRandomString(4) + GenerateRandomNo() + DateTime.Now.Millisecond.ToString() + j + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                                acode.Rows.Add(AC.ProductId, authcode, DateTime.Now, "BLK", lastId, validity);
                                System.Threading.Thread.Sleep(1);
                            }

                            if (acode.Rows.Count > 0)
                            {
                                //insert data in bulk copy
                                using (var bulkcopy = new SqlBulkCopy(con, SqlBulkCopyOptions.KeepIdentity, transaction))
                                {
                                    foreach (DataColumn dc in acode.Columns)
                                    {
                                        bulkcopy.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                                    }
                                    bulkcopy.BulkCopyTimeout = 100000;
                                    bulkcopy.DestinationTableName = "tblAuthcode";
                                    bulkcopy.WriteToServer(acode);
                                }
                            }
                            //data commit
                            transaction.Commit();

                        }

                    }
                    return true;
                }

            }
            catch (Exception ex)
            {
             //   transaction.Rollback();
                ex.ToString();
                return false;
            }
        }
        //Generate a Random number into 1000 to 9999
        private int GenerateRandomNo()
        {
            Random _rdm = new Random();
            return _rdm.Next(1000, 9999);
        }

        //Generate a Random string into 4 digit
        private string GenerateRandomString(int size)
        {
            try
            {
                Random random = new Random((int)DateTime.Now.Ticks);
                StringBuilder builder = new StringBuilder();
                char ch;
                for (int i = 0; i < size; i++)
                {
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                    builder.Append(ch);
                }
                return builder.ToString().ToUpper();
            }
            catch (Exception ex)
            {
                //Log.Error("" + ex);
                return null;
            }
        }

        public List<SelectListItem> GetProductDetails()
        {
            try
            {
                List<SelectListItem> li = new List<SelectListItem>();
                using (SqlConnection con = new SqlConnection(strcon))
                {

                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetProductList");
                  
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        li.Add(new SelectListItem
                        {
                            Text = dr["Text"].ToString(),
                            Value = dr["Id"].ToString()
                        });
                    }
                    con.Close();
                }
                return li; //
                           //  return new SelectList(li, "Value", "Text");
                           //return li;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

        public List<UserDetail> GetCDFListAssignToStudByPincode(int CandId,int pinCode)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetCDFByPincode");
                    cmd.Parameters.AddWithValue("@pinCode", pinCode);
                    cmd.Parameters.AddWithValue("@StudentId", CandId);

                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            UserDetail UD = new UserDetail();
                            UD.CDFAvailableMsg = "1";// "CDF is Already Assigned to Candidate...";
                            li.Add(UD);
                        }
                        else
                        {
                            // while(ds.Tables[1].Rows.Count > 0)
                            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                            {

                                UserDetail UD = new UserDetail();
                                UD.CDFAvailableMsg = "2";
                                UD.CDFId = Convert.ToInt32(ds.Tables[1].Rows[i]["uId"]);
                                UD.CDFName = ds.Tables[1].Rows[i]["CDFName"].ToString();
                                UD.CDFGender = ds.Tables[1].Rows[i]["gender"].ToString();
                                UD.CDFContact = ds.Tables[1].Rows[i]["contactNo"].ToString();
                                UD.CDFEmail = ds.Tables[1].Rows[i]["email"].ToString();
                                int CDFLevel = Convert.ToInt32(ds.Tables[1].Rows[i]["CDFLevel"]);
                                string CDFLvl = "L" + CDFLevel;
                                UD.CDFLevels = CDFLvl;
                                UD.CDFAddress = ds.Tables[1].Rows[i]["address"].ToString();
                                //UD.CDFPinCode = sdr["pincode"].ToString();     
                                UD.CDFPinCode = ds.Tables[1].Rows[i]["pincode"].ToString();
                                UD.CDFCity = ds.Tables[1].Rows[i]["name"].ToString();
                                UD.CDFSesDate = ds.Tables[1].Rows[i]["CDFSesDate"].ToString();
                                UD.CDFSessionCnt = Convert.ToInt32(ds.Tables[1].Rows[i]["SessionCount"]);
                                li.Add(UD);
                            }
                        }


                    }

                    //SqlDataReader sdr = cmd.ExecuteReader();
                    //if (sdr.HasRows)
                    //{
                    //    while (sdr.Read())
                    //    {
                    //        UserDetail UD = new UserDetail();


                    //        UD.CDFId = Convert.ToInt32(sdr["uId"]);
                    //        UD.CDFName = sdr["CDFName"].ToString();
                    //        UD.CDFGender = sdr["gender"].ToString();
                    //        UD.CDFContact = sdr["contactNo"].ToString();
                    //        UD.CDFEmail = sdr["email"].ToString();
                    //        int CDFLevel = Convert.ToInt32(sdr["CDFLevel"]);
                    //        string CDFLvl = "L" + CDFLevel;
                    //        UD.CDFLevels = CDFLvl;
                    //        UD.CDFAddress = sdr["address"].ToString();
                    //        //UD.CDFPinCode = sdr["pincode"].ToString();     
                    //        UD.CDFPinCode = sdr["pincode"].ToString();


                    //        li.Add(UD);


                    //    }
                    //}
                    return li;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<UserDetail> GetCDFListAssignToStudByEmailId(int CandId, string EmailId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetCDFByPincode");
                    cmd.Parameters.AddWithValue("@email", EmailId);
                    cmd.Parameters.AddWithValue("@StudentId", CandId);

                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            UserDetail UD = new UserDetail();
                            UD.CDFAvailableMsg = "1";// "CDF is Already Assigned to Candidate...";
                            li.Add(UD);
                        }
                        else
                        {
                            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                            {
                                UserDetail UD = new UserDetail();
                                UD.CDFAvailableMsg = "2";
                                UD.CDFId = Convert.ToInt32(ds.Tables[1].Rows[i]["uId"]);
                                UD.CDFName = ds.Tables[1].Rows[i]["CDFName"].ToString();
                                UD.CDFGender = ds.Tables[1].Rows[i]["gender"].ToString();
                                UD.CDFContact = ds.Tables[1].Rows[i]["contactNo"].ToString();
                                UD.CDFEmail = ds.Tables[1].Rows[i]["email"].ToString();
                                int CDFLevel = Convert.ToInt32(ds.Tables[1].Rows[i]["CDFLevel"]);
                                string CDFLvl = "L" + CDFLevel;
                                UD.CDFLevels = CDFLvl;
                                UD.CDFAddress = ds.Tables[1].Rows[i]["address"].ToString();
                                //UD.CDFPinCode = sdr["pincode"].ToString();     
                                UD.CDFPinCode = ds.Tables[1].Rows[i]["pincode"].ToString();
                                UD.CDFCity = ds.Tables[1].Rows[i]["name"].ToString();
                                UD.CDFSesDate = ds.Tables[1].Rows[i]["CDFSesDate"].ToString();
                                UD.CDFSessionCnt = Convert.ToInt32(ds.Tables[1].Rows[i]["SessionCount"]);
                                li.Add(UD);
                            }
                        }
                    }
                    return li;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<UserDetail> GetCDFListAssignToStudByCity(int CandId, string CityName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetCDFByPincode");
                    cmd.Parameters.AddWithValue("@CityNm", CityName);
                    cmd.Parameters.AddWithValue("@StudentId", CandId);

                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            UserDetail UD = new UserDetail();
                            UD.CDFAvailableMsg = "1";// "CDF is Already Assigned to Candidate...";
                            li.Add(UD);
                        }
                        else
                        {
                            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                            {
                                UserDetail UD = new UserDetail();
                                UD.CDFAvailableMsg = "2";
                                UD.CDFId = Convert.ToInt32(ds.Tables[1].Rows[i]["uId"]);
                                UD.CDFName = ds.Tables[1].Rows[i]["CDFName"].ToString();
                                UD.CDFGender = ds.Tables[1].Rows[i]["gender"].ToString();
                                UD.CDFContact = ds.Tables[1].Rows[i]["contactNo"].ToString();
                                UD.CDFEmail = ds.Tables[1].Rows[i]["email"].ToString();
                                int CDFLevel = Convert.ToInt32(ds.Tables[1].Rows[i]["CDFLevel"]);
                                string CDFLvl = "L" + CDFLevel;
                                UD.CDFLevels = CDFLvl;
                                UD.CDFAddress = ds.Tables[1].Rows[i]["address"].ToString();
                                //UD.CDFPinCode = sdr["pincode"].ToString();     
                                UD.CDFPinCode = ds.Tables[1].Rows[i]["pincode"].ToString();
                                UD.CDFCity = ds.Tables[1].Rows[i]["name"].ToString();
                                UD.CDFSesDate = ds.Tables[1].Rows[i]["CDFSesDate"].ToString();
                                UD.CDFSessionCnt = Convert.ToInt32(ds.Tables[1].Rows[i]["SessionCount"]);
                                li.Add(UD);
                            }
                        }
                    }
                    return li;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<SelectListItem> Get_City()
        {
            try
            {
                List<SelectListItem> li = new List<SelectListItem>();
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_CDFCities", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        li.Add(new SelectListItem
                        {
                            Text = dr["CityName"].ToString(),
                            Value = dr["CityName"].ToString()
                        });
                    }
                    con.Close();
                }
                return li; //
                           //  return new SelectList(li, "Value", "Text");
                           //return li;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        public List<UserDetail> GetShadowCDFListToAssignStudent(string candPinCode, int CandId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetShadowCDFToAssign");
                    cmd.Parameters.AddWithValue("@pinCode", candPinCode);
                    cmd.Parameters.AddWithValue("@StudentId", CandId);

                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            UserDetail UD = new UserDetail();
                            UD.ShadowCDFAvailableMsg = "1";// "Shadow CDF is Already Assigned to Candidate...";
                            li.Add(UD);
                        }
                        else
                        {
                            // while(ds.Tables[1].Rows.Count > 0)
                            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                            {

                                UserDetail UD = new UserDetail();
                                UD.ShadowCDFAvailableMsg = "2";
                                UD.CDFId = Convert.ToInt32(ds.Tables[1].Rows[i]["uId"]);
                                UD.CDFName = ds.Tables[1].Rows[i]["CDFName"].ToString();
                                UD.CDFGender = ds.Tables[1].Rows[i]["gender"].ToString();
                                UD.CDFContact = ds.Tables[1].Rows[i]["contactNo"].ToString();
                                UD.CDFEmail = ds.Tables[1].Rows[i]["email"].ToString();
                                int CDFLevel = Convert.ToInt32(ds.Tables[1].Rows[i]["CDFLevel"]);
                                string CDFLvl = "L" + CDFLevel;
                                UD.CDFLevels = CDFLvl;
                                UD.CDFAddress = ds.Tables[1].Rows[i]["address"].ToString();
                                UD.CDFPinCode = ds.Tables[1].Rows[i]["pincode"].ToString();
                                //UD.CDFSesDate = ds.Tables[1].Rows[i]["CDFSesDate"].ToString();
                                UD.CDFCity = ds.Tables[1].Rows[i]["name"].ToString();
                                li.Add(UD);
                            }
                        }


                    }

                    //    SqlDataReader sdr = cmd.ExecuteReader();
                    //    if (sdr.HasRows)
                    //    {
                    //        while (sdr.Read())
                    //        {
                    //            UserDetail UD = new UserDetail();


                    //            UD.CDFId = Convert.ToInt32(sdr["uId"]);
                    //            UD.CDFName = sdr["CDFName"].ToString();
                    //            UD.CDFGender = sdr["gender"].ToString();
                    //            UD.CDFContact = sdr["contactNo"].ToString();
                    //            UD.CDFEmail = sdr["email"].ToString();
                    //            int level = Convert.ToInt32(sdr["CDFLevel"]);
                    //            string CDFLevel = "L" + level;
                    //            UD.CDFLevels = CDFLevel;
                    //            UD.CDFAddress = sdr["address"].ToString();
                    //            UD.CDFPinCode = sdr["pincode"].ToString();


                    //            li.Add(UD);


                    //        }
                    //    }
                    return li;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int CandidateTestReassign(int testId, int testCompCandId)
        {
            try
            {
                int i = 0;
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    string sqlStat = "select count(*) as testCount from tblUserTestMaster where Cid='" + testCompCandId + "' AND  testid='" + testId + "'";
                    SqlCommand cmd101 = new SqlCommand(sqlStat, con);
                    con.Open();
                    int CandTestCunt = 0;
                    SqlDataReader dr101 = cmd101.ExecuteReader();
                    if (dr101.HasRows)
                    {
                        dr101.Read();
                        CandTestCunt = Convert.ToInt32(dr101["testCount"]);
                    }
                    dr101.Close();
                    if (CandTestCunt > 0 || testId == 101 || testId == 102)
                    {
                        if (testId == 8)
                        {
                            SqlCommand cmd = new SqlCommand("sp_ReassignTest", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@CandId", testCompCandId);
                            cmd.Parameters.AddWithValue("@Type", "Interest");
                           // con.Open();
                            i = cmd.ExecuteNonQuery();

                            //if (i > 0)
                            //{
                            //    return true;
                            //}
                            //else
                            //{

                            //    return false;
                            //}
                        }
                        if (testId == 10)
                        {
                            SqlCommand cmd1 = new SqlCommand("sp_ReassignTest", con);
                            cmd1.CommandType = CommandType.StoredProcedure;
                            cmd1.Parameters.AddWithValue("@CandId", testCompCandId);
                            cmd1.Parameters.AddWithValue("@Type", "PD");
                         //   con.Open();
                            i = cmd1.ExecuteNonQuery();
                        }
                        if (testId == 9)
                        {
                            SqlCommand cmd1 = new SqlCommand("sp_ReassignTest", con);
                            cmd1.CommandType = CommandType.StoredProcedure;
                            cmd1.Parameters.AddWithValue("@CandId", testCompCandId);
                            cmd1.Parameters.AddWithValue("@Type", "KY");
                          //  con.Open();
                            i = cmd1.ExecuteNonQuery();

                        }

                        if (testId == 1 || testId == 2 || testId == 3 || testId == 4 || testId == 5 || testId == 6 || testId == 7)
                        {
                            GetAbilityTAnsCount();
                            i = test1to5and7(testId, testCompCandId);

                        }
                        if (testId == 101)
                        {
                            i = reassignAllAbilityTest(testCompCandId);
                        }
                        if (testId == 102)
                        {
                            i = reassignAllTest(testCompCandId);
                        }


                    }
                    else {
                        i = 100100100;
                    }
                            
                        
                    return i;

                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public void GetAbilityTAnsCount()
        {
            try
            {
                int c_Id = 82;
                string strcmd="";
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    con.Open();
                    strcmd = "select count(q_id) as cnt from tblAbilityCandAnswers where c_id = '" + c_Id + "'  and(q_id  between 1 and 15) group by c_id";
                    SqlCommand cmd = new SqlCommand(strcmd,con);
                    int q1;
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        q1 = Convert.ToInt32(dr["cnt"]);
                    }
                    else
                    {
                        q1 = 0;
                       
                    }
                    dr.Close();
                    string q1_completed_que = q1.ToString();
                    string q1_remin_que = (15 - q1).ToString();

                    strcmd = "select count(q_id) as cnt from tblAbilityCandAnswers where c_id = '" + c_Id + "' and(q_id  between 16 and 30) group by c_id";
                    SqlCommand cmd1 = new SqlCommand(strcmd, con);
                    int q2;
                    SqlDataReader dr1 = cmd1.ExecuteReader();
                    if (dr1.HasRows)
                    {
                        dr1.Read();
                        q2 = Convert.ToInt32(dr1["cnt"]);
                    }
                    else
                    {
                        q2 = 0;
                      
                    }
                    dr1.Close();
                    string q2_completed_que = q1.ToString();
                    string q2_remin_que = (15 - Convert.ToInt32(q1)).ToString();

                    strcmd = "select count(q_id) as cnt from tblAbilityCandAnswers where c_id= '" + c_Id + "' and (q_id  between 31 and 45) group by c_id";
                    SqlCommand cmd2 = new SqlCommand(strcmd, con);
                    int q3;
                    SqlDataReader dr2 = cmd2.ExecuteReader();
                    if (dr2.HasRows)
                    {
                        dr2.Read();
                        q3 = Convert.ToInt32(dr2["cnt"]);
                    }
                    else
                    {
                        q3 = 0;

                    }
                    dr2.Close();
                    string q3_completed_que = q3.ToString();
                    string q3_remin_que = (15 - Convert.ToInt32(q3)).ToString();

                    strcmd = "select count(q_id) as cnt from tblAbilityCandAnswers where c_id ='" + c_Id + "' and (q_id  between 46 and 60) group by c_id";
                    SqlCommand cmd3 = new SqlCommand(strcmd, con);
                    int q4;
                    SqlDataReader dr3 = cmd3.ExecuteReader();
                    if (dr3.HasRows)
                    {
                        dr3.Read();
                        q4 = Convert.ToInt32(dr3["cnt"]);
                    }
                    else
                    {
                        q4 = 0;

                    }
                    dr3.Close();
                    string q4_completed_que = q4.ToString();
                    string q4_remin_que = (15 - Convert.ToInt32(q4)).ToString();

                    strcmd = "select count(q_id) as cnt from  tblAbilityCandAnswers where c_id ='" + c_Id + "' and (q_id  between 61 and 75) group by c_id";
                    SqlCommand cmd4 = new SqlCommand(strcmd, con);
                    int q5;
                    SqlDataReader dr4 = cmd4.ExecuteReader();
                    if (dr4.HasRows)
                    {
                        dr4.Read();
                        q5 = Convert.ToInt32(dr4["cnt"]);
                    }
                    else
                    {
                        q5 = 0;

                    }
                    dr4.Close();
                    string q5_completed_que = q5.ToString();
                    string q5_remin_que = (15 - Convert.ToInt32(q5)).ToString();


                    strcmd = "SELECT COUNT(q_id) as cnt FROM tblMRCandAnswers  where c_id='" + c_Id + "'";
                    SqlCommand cmd5 = new SqlCommand(strcmd, con);
                    int q6;
                    SqlDataReader dr5 = cmd5.ExecuteReader();
                    if (dr5.HasRows)
                    {
                        dr5.Read();
                        q6 = Convert.ToInt32(dr5["cnt"]);
                    }
                    else
                    {
                        q6 = 0;

                    }
                    dr5.Close();
                    string q6_completed_que = q6.ToString();
                    string q6_remin_que = (3 - Convert.ToInt32(q6)).ToString();

                    strcmd = "select count(q_id) as cnt from tblAbilityCandAnswers where c_id ='" + c_Id + "' and (q_id  between 79 and 93) group by c_id";
                    SqlCommand cmd6 = new SqlCommand(strcmd, con);
                    int q7;
                    SqlDataReader dr6 = cmd6.ExecuteReader();
                    if (dr6.HasRows)
                    {
                        dr6.Read();
                        q7 = Convert.ToInt32(dr6["cnt"]);
                    }
                    else
                    {
                        q7 = 0;

                    }
                    dr6.Close();
                    string q7_completed_que = q7.ToString();
                    string q7_remin_que = (15 - Convert.ToInt32(q7)).ToString();

                    //Interest Test

                    strcmd = "SELECT COUNT(q_no) as cnt FROM tblInterestCandAnswers where c_id='" + c_Id + "'";
                    SqlCommand cmd7 = new SqlCommand(strcmd, con);
                    int inte;

                    SqlDataReader dr7 = cmd7.ExecuteReader();
                    if (dr7.HasRows)
                    {
                        dr7.Read();
                        inte = Convert.ToInt32(dr7["cnt"]);
                    }
                    else
                    {
                        inte = 0;

                    }
                    dr7.Close();
                    string int_completed_que = inte.ToString();
                    string int_remin_que = (128 - Convert.ToInt32(inte)).ToString();
                    string int_status = "";
                    string int_factor = "";
                    if (int_completed_que == "128")
                    {
                        strcmd = "";
                        strcmd = "SELECT count(factor_no) as cnt FROM tblInterestCandFactors where c_id = '" + c_Id + "'";
                        SqlCommand cmd8 = new SqlCommand(strcmd, con);

                        SqlDataReader dr8 = cmd8.ExecuteReader();
                        if (dr8.HasRows)
                        {

                            dr8.Read();
                             int_status = "";
                             int_factor = dr8["cnt"].ToString();
                            if (int_factor == "16")
                            {
                                int_status = "Complete";
                            }
                            else
                            {
                                int_status = "Factor InComplete";
                            }

                            int_factor = "" + dr8["cnt"].ToString();
                        }
                        else
                        {
                            int_status = "InComplete";
                            int_factor = "No Factor";
                        }
                        dr8.Close();
                    }
                    else
                    {
                        int_status = "InComplete";
                        int_factor = "No Factor";
                    }
                    


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int test1to5and7(int Inttestvalue, int c_id)
        {
            string StrQuery = "";
            int i = 0;
            using (SqlConnection con = new SqlConnection(strcon))
            {
                //ability_1-5&7
              
                con.Open();
                string strQuery = "Insert Into tbl_test_reasign(c_id,test_no,adminid,date,remark) values (" + c_id + " , " + Inttestvalue + ",'" + "Admin" + "','" + System.DateTime.Now.Date + "','" + "No" + "')";
                SqlCommand cmd = new SqlCommand(strQuery,con);
                i = cmd.ExecuteNonQuery();

                strQuery = "INSERT Into tblAbilityCandAnswersHistory (c_id,q_id,selectedOption,result,marks,timeTaken,batid) " +
                            "SELECT A.c_id,A.q_id,A.selectedOption,A.result,A.marks,A.timeTaken,A.batid FROM tblAbilityCandAnswers A " +
                            "INNER JOIN  tblTestDetails AS B ON A.q_id = B.q_id WHERE c_id = " + c_id + " AND B.test_id = " + Inttestvalue + "";
                SqlCommand cmd1 = new SqlCommand(strQuery,con);
                i = cmd1.ExecuteNonQuery();

                strQuery = "DELETE FROM tblAbilityCandAnswers WHERE c_id = " + c_id + " AND q_id in (SELECT q_id FROM tblTestDetails WHERE test_id= " + Inttestvalue + ")";
                SqlCommand cmd2 = new SqlCommand(strQuery, con);
                i = cmd2.ExecuteNonQuery();

                strQuery= "DELETE FROM  tblAbilityCandFactors WHERE c_id = " + c_id + " and testid=" + Inttestvalue ;
                SqlCommand cmd3 = new SqlCommand(strQuery,con);
                i = cmd3.ExecuteNonQuery();

                strQuery = "DELETE FROM tblUserTestMaster WHERE Cid=" + c_id + " AND testid=" + Inttestvalue + "";
                SqlCommand cmd4 = new SqlCommand(strQuery, con);
                i = cmd4.ExecuteNonQuery();

                strQuery= "UPDATE tblCandidateProductMaster SET teststatus ='Reassign' WHERE CId=" + c_id + "";
                //, dateoftestcomplete = null
                SqlCommand cmd5 = new SqlCommand(strQuery, con);
                i = cmd5.ExecuteNonQuery();
                return i;
            }
          
        }

        public int reassignAllAbilityTest(int c_id)
        {
            int i;
            try {

                int p;
                string StrQuery = "";
            
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    con.Open();
                    for (p = 0; p < 7; p++)
                    {
                        StrQuery = "Insert Into tbl_test_reasign(c_id,test_no,adminid,date,remark) values (" + c_id + " , " + (p + 1) + ",'" + "Admin" + "','" + System.DateTime.Now.Date + "','" + "No" + "')";
                        SqlCommand cmd = new SqlCommand(StrQuery, con);
                        i = cmd.ExecuteNonQuery();
                      
                    }
                        // All Ability Test
                        for (int g = 1; g <= 7; g++)
                        {
                            if (g != 6)
                            {
                                StrQuery = "INSERT Into tblAbilityCandAnswersHistory (c_id,q_id,selectedOption,result,marks,timeTaken,batid) " +
                                        "SELECT A.c_id,A.q_id,A.selectedOption,A.result,A.marks,A.timeTaken,A.batid FROM tblAbilityCandAnswers A " +
                                        "INNER JOIN  tblTestDetails AS B ON A.q_id = B.q_id WHERE c_id = " + c_id + " AND B.test_id = " + g + "";

                                SqlCommand cmd1 = new SqlCommand(StrQuery, con);
                                i = cmd1.ExecuteNonQuery();

                                StrQuery = "DELETE FROM tblAbilityCandAnswers WHERE c_id = " + c_id + " " +
                                            "AND q_id in (SELECT q_id FROM tblTestDetails WHERE test_id = " + g + ")";
                                SqlCommand cmd2 = new SqlCommand(StrQuery, con);
                                i = cmd2.ExecuteNonQuery();

                                StrQuery = "DELETE FROM tblAbilityCandFactors WHERE c_id = " + c_id + " and testid=" + g + "";
                                SqlCommand cmd3 = new SqlCommand(StrQuery, con);
                                i = cmd3.ExecuteNonQuery();

                            StrQuery = "DELETE FROM tblUserTestMaster WHERE Cid = " + c_id + " and testid=" + g + "";
                            SqlCommand cmd444 = new SqlCommand(StrQuery, con);
                            i = cmd444.ExecuteNonQuery();

                          


                        }
                            else
                            {
                                StrQuery = "INSERT Into tblMRCandAnswersHistory(c_id,q_id,selectedopt1,selectedopt2,selectedopt3,selectedopt4,selectedopt5,marks,timeTaken,batid)"+ 
                                "SELECT c_id, q_id, selectedopt1, selectedopt2, selectedopt3, selectedopt4, selectedopt5, marks, timeTaken, batid FROM tblMRCandAnswers WHERE c_id = " + c_id ;
                                SqlCommand cmd4 = new SqlCommand(StrQuery,con);
                                i = cmd4.ExecuteNonQuery();

                                StrQuery = "DELETE FROM tblMRCandAnswers WHERE c_id = " + c_id + "";
                                SqlCommand cmd5 = new SqlCommand(StrQuery,con);
                                i = cmd5.ExecuteNonQuery();

                                StrQuery= "DELETE FROM  tblAbilityCandFactors WHERE c_id = " + c_id + " and testid=" + g;
                                SqlCommand cmd6 = new SqlCommand(StrQuery,con);
                                i = cmd6.ExecuteNonQuery();

                                StrQuery = "DELETE FROM tblUserTestMaster WHERE Cid=" + c_id + " AND testid=" + g + "";
                                SqlCommand cmd7 = new SqlCommand(StrQuery,con);
                                i = cmd7.ExecuteNonQuery();

                                StrQuery = "DELETE FROM  tblAbilityCandFactors WHERE c_id = " + c_id + " and testid=" + g;
                                SqlCommand cmd8 = new SqlCommand(StrQuery,con);
                                i = cmd8.ExecuteNonQuery();

                            StrQuery = "DELETE FROM tblUserTestMaster WHERE Cid = " + c_id + " and testid=" + g + "";
                            SqlCommand cmd404 = new SqlCommand(StrQuery, con);
                            i = cmd404.ExecuteNonQuery();


                        }

                        }

                    StrQuery = "UPDATE tblCandidateProductMaster SET teststatus ='Reassign' WHERE CId=" + c_id + "";
                    SqlCommand cmd9 = new SqlCommand(StrQuery,con);
                    i = cmd9.ExecuteNonQuery();
                    return i;
                }
               
            }
            catch (Exception ex)
            {
                ex.ToString();
                return 1010101;
            }
        }

        public int reassignAllTest(int c_id)
        {
          
            try {
                string StrQuery = "";
                int i = 0;
               // c_id = c_id;
                string remark = "";
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    con.Open();
                    for (int p = 0; p < 10; p++)
                    {
                        StrQuery = "Insert Into tbl_test_reasign(c_id,test_no,adminid,date,remark) values (" + c_id + " , " + (p + 1) + ",'" + "Admin" + "','" + System.DateTime.Now.Date + "','" + "No" + "')";
                        SqlCommand cmd = new SqlCommand(StrQuery, con);
                        i = cmd.ExecuteNonQuery();
                    }
                    //All Test
                    // For Test 1 to 5 and 7 th test
                    for (int g = 1; g <= 7; g++)
                    {
                        if (g != 6)
                        {
                            StrQuery = "INSERT Into tblAbilityCandAnswersHistory (c_id,q_id,selectedOption,result,marks,timeTaken,batid) " +
                                       "SELECT A.c_id,A.q_id,A.selectedOption,A.result,A.marks,A.timeTaken,A.batid FROM tblAbilityCandAnswers A " +
                                       "INNER JOIN  tblTestDetails AS B ON A.q_id = B.q_id WHERE c_id = " + c_id + " AND B.test_id = " + g + "";

                            SqlCommand cmd1 = new SqlCommand(StrQuery, con);
                            i = cmd1.ExecuteNonQuery();

                            StrQuery = "DELETE FROM tblAbilityCandAnswers WHERE c_id = " + c_id + " " +
                                        "AND q_id in (SELECT q_id FROM tblTestDetails WHERE test_id = " + g + ")";
                            SqlCommand cmd2 = new SqlCommand(StrQuery, con);
                            i = cmd2.ExecuteNonQuery();

                            StrQuery = "DELETE FROM tblAbilityCandFactors WHERE c_id = " + c_id + " and testid=" + g + "";
                            SqlCommand cmd3 = new SqlCommand(StrQuery, con);
                            i = cmd3.ExecuteNonQuery();

                         
                        }
                        else
                        {
                            // For 6th Test

                            StrQuery = "INSERT Into tblMRCandAnswersHistory(c_id,q_id,selectedopt1,selectedopt2,selectedopt3,selectedopt4,selectedopt5,marks,timeTaken,batid)" +
                                   "SELECT c_id, q_id, selectedopt1, selectedopt2, selectedopt3, selectedopt4, selectedopt5, marks, timeTaken, batid FROM tblMRCandAnswers WHERE c_id = " + c_id;
                            SqlCommand cmd4 = new SqlCommand(StrQuery, con);
                            i = cmd4.ExecuteNonQuery();

                            StrQuery = "DELETE FROM tblMRCandAnswers WHERE c_id = " + c_id + "";
                            SqlCommand cmd5 = new SqlCommand(StrQuery, con);
                            i = cmd5.ExecuteNonQuery();

                            StrQuery = "DELETE FROM  tblAbilityCandFactors WHERE c_id = " + c_id + " and testid=" + g;
                            SqlCommand cmd6 = new SqlCommand(StrQuery, con);
                            i = cmd6.ExecuteNonQuery();

                          
                        }
                        if (i > 0)
                        {
                            //if (g == 1 || g == 2 || g == 3 || g == 4 || g == 5 || g == 6 || g == 7)
                            //{
                            //    StrQuery = "UPDATE tbl_candidate_tb_master SET c_tb_status ='Not Complete', total_test_completed = " + (g - 1) + " WHERE c_id=" + c_id + "";
                            //    i = clsdal.ExecNonQuery(StrQuery);
                            //}

                            StrQuery = "DELETE FROM tblUserTestMaster WHERE Cid=" + c_id + " AND testid=" + g + "";
                            SqlCommand cmd7 = new SqlCommand(StrQuery, con);
                            i = cmd7.ExecuteNonQuery();

                            StrQuery = "DELETE FROM  tblAbilityCandFactors WHERE c_id = " + c_id + " and testid=" + g;
                            SqlCommand cmd8 = new SqlCommand(StrQuery, con);
                            i = cmd8.ExecuteNonQuery();
                        }

                        }

                    // For 8th Test

                    StrQuery = "insert into tblInterestCandAnswersHistory([c_id],[q_no],[factor_no],[marks],[batid])"+
                            " select[c_id],[q_no],[factor_no],[marks],[batid] from tblInterestCandAnswers where c_id =" + c_id;
                    SqlCommand cmd9 = new SqlCommand(StrQuery, con);
                    i = cmd9.ExecuteNonQuery();

                    StrQuery = "delete from tblInterestCandAnswers where c_id=" + c_id;
                    SqlCommand cmd10 = new SqlCommand(StrQuery, con);
                    i = cmd10.ExecuteNonQuery();

                    StrQuery = "insert into tblInterestCandFactorsHistory([c_id],[factor_no],[score],[rating],[P_rating],[batid])"+
                            " select[c_id],[factor_no],[score],[rating],[P_rating],[batid] from tblInterestCandFactors where c_id =" + c_id;
                    SqlCommand cmd11 = new SqlCommand(StrQuery, con);
                    i = cmd11.ExecuteNonQuery();

                    StrQuery = "delete from tblInterestCandFactors where c_id= " + c_id;
                    SqlCommand cmd12 = new SqlCommand(StrQuery, con);
                    i = cmd12.ExecuteNonQuery();

                    StrQuery = "DELETE FROM tblUserTestMaster WHERE Cid=" + c_id + " AND testid=8";
                    SqlCommand cmd1001 = new SqlCommand(StrQuery, con);
                    i = cmd1001.ExecuteNonQuery();

                    // For 9th Test

                    StrQuery = "insert into tblKYCandAnswersHistory1(c_id,ParentId,q_no,factor_no,marks,batid)"+
                                " select c_id, ParentId, q_no, factor_no, marks, batid from tblKYCandAnswers1 where c_id = " + c_id;
                    SqlCommand cmd13 = new SqlCommand(StrQuery, con);
                    i = cmd13.ExecuteNonQuery();

                    StrQuery = "delete from tblKYCandAnswers1 where c_id=" + c_id;
                    SqlCommand cmd14 = new SqlCommand(StrQuery, con);
                    i = cmd14.ExecuteNonQuery();

                    StrQuery = "insert into tblKYCandFactorsHistory1 (id,c_id,factor_no,score,rating,P_rating,measure,New_P_rating,batid)"+
                    " select id, c_id, factor_no, score, rating, P_rating, measure, New_P_rating, batid from tblKYCandFactors1 where c_id = " + c_id;
                    SqlCommand cmd15 = new SqlCommand(StrQuery, con);
                    i = cmd15.ExecuteNonQuery();

                    StrQuery = "delete from tblKYCandFactors1 where c_id= " + c_id;
                    SqlCommand cmd16 = new SqlCommand(StrQuery, con);
                    i = cmd16.ExecuteNonQuery();

                    StrQuery = "DELETE FROM tblUserTestMaster WHERE Cid=" + c_id + " AND testid=9";
                    SqlCommand cmd1002 = new SqlCommand(StrQuery, con);
                    i = cmd1002.ExecuteNonQuery();

                    // For 10th Test

                    StrQuery = "insert into tblPersonalityCandAnswersHistory1(c_id,q_id,most_qno,least_qno,most_code,least_code,most_status,least_status,batid)"+
                    "select c_id, q_id, most_qno, least_qno, most_code, least_code, most_status, least_status, batid from tblPersonalityCandAnswers1 where c_id = " + c_id;
                    SqlCommand cmd17 = new SqlCommand(StrQuery, con);
                    i = cmd17.ExecuteNonQuery();

                    StrQuery = "delete from tblPersonalityCandAnswers1 where c_id=" + c_id;
                    SqlCommand cmd18 = new SqlCommand(StrQuery, con);
                    i = cmd18.ExecuteNonQuery();

                    StrQuery = "DELETE FROM tblUserTestMaster WHERE Cid=" + c_id + " AND testid=10";
                    SqlCommand cmd1003 = new SqlCommand(StrQuery, con);
                    i = cmd1003.ExecuteNonQuery();

                    if (i > 0)
                    {
                        StrQuery = "DELETE FROM tblUserTestMaster WHERE Cid=" + c_id + " AND testid= 11";
                        SqlCommand cmd19 = new SqlCommand(StrQuery, con);
                        i = cmd19.ExecuteNonQuery();

                        StrQuery = "UPDATE tblAbilityCandFactors SET marks=0 WHERE c_id=" + c_id + " AND testid=11";
                        SqlCommand cmd20 = new SqlCommand(StrQuery, con);
                        i = cmd20.ExecuteNonQuery();
                    }

                    StrQuery = "UPDATE tblCandidateProductMaster SET teststatus ='Reassign' WHERE CId=" + c_id + "";
                    SqlCommand cmd21 = new SqlCommand(StrQuery, con);
                    i = cmd21.ExecuteNonQuery();
                }
                return i;

            }
            catch (Exception ex)
            {
                ex.ToString();
                return 1010101;
            }
           
        }

        public List<SelectListItem> GetDDLAllTest(int candIdTOGetTest)
        {
            try
            {
                List<SelectListItem> li = new List<SelectListItem>();
                using (SqlConnection con = new SqlConnection(strcon))
                {

                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetAllTest");
                    cmd.Parameters.AddWithValue("@StudentId", candIdTOGetTest);
                   
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        li.Add(new SelectListItem
                        {
                            Text = dr["test_name"].ToString(),
                            Value = dr["test_ID"].ToString()
                        });
                    }
                    con.Close();
                }
                return li; 
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

        public List<UserDetail> GetUserFeedbackDtl()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_userfeedback", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "FeedbackDtl");
                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            UserDetail UD = new UserDetail();
                            UD.Id = Convert.ToInt32(sdr["Id"]);
                            UD.UserType = sdr["UserType"].ToString();
                            UD.Name = sdr["Name"].ToString();
                            UD.EmailId = sdr["email"].ToString();
                            UD.Gender = sdr["Gender"].ToString();
                            UD.Contact = sdr["Contactno"].ToString();
                            UD.Address = sdr["Address"].ToString();
                            UD.Pincode = sdr["Pincode"].ToString();
                            UD.UserTypeId = Convert.ToInt32(sdr["UsertypeId"]);
                            UD.ParentFb = sdr["ParentFb"].ToString();
                            li.Add(UD);
                        }
                    }
                    return li;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetSPPFeedbackCount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    int SPPFeedbackCount = 0;
                    SqlCommand cmd = new SqlCommand("sp_userfeedback", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "FeedbackDtlCount");
                    UserDetail UD = new UserDetail();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        SPPFeedbackCount = Convert.ToInt32(sdr["FeedbackDtlCnt"]);
                    }
                    return SPPFeedbackCount;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CommnetAndRating> GetComRatingFdb(int UId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                   
                    SqlCommand cmd = new SqlCommand("select top 10 * from tblCommentsNRating where User_Id='" + UId + "'", con);
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                       List<CommnetAndRating> li = new List<CommnetAndRating>();
                        while (sdr.Read())
                        {
                            CommnetAndRating cmt = new CommnetAndRating();
                            cmt.Id = Convert.ToInt32(sdr["User_Id"]);
                            cmt.UserTypeId = Convert.ToInt32(sdr["UserTypeId"]);
                            cmt.Comment = sdr["Comment"].ToString();
                            cmt.Rating = Convert.ToInt32(sdr["Rating"]);
                            li.Add(cmt);
                        }
                        
                      return li;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<FeedBackDetail> GetFeedBackQuestions(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("select sfq.Ques_no as Quesno,sfq.Ques_text as QuesText,spfa.Ques_Answer as AnsText from tblStudentFeedbackQuestions as sfq inner join tblStudentProfessionalFeedbackAns as spfa on sfq.Ques_no=spfa.Ques_no where spfa.User_Id='" + id + "'", con);
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        int i = 1;
                        List<FeedBackDetail> questions = new List<FeedBackDetail>();
                        while (sdr.Read())
                        {
                            FeedBackDetail PF = new FeedBackDetail();
                            PF.Qid = i++;
                            PF.Qno = Convert.ToInt32(sdr["Quesno"]);
                            PF.Questiontxt = sdr["QuesText"].ToString();
                            PF.Answertxt = sdr["AnsText"].ToString();
                            questions.Add(PF);
                        }
                        return questions;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FeedBackDetailDtl> GetFeedBackQuestionsDtl(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("select sfq.Ques_no as Quesno,sfq.Ques_text as QuesText,spfa.Ques_Answer as AnsText from tblStudentFeedbackQuestions as sfq inner join tblStudentProfessionalFeedbackAns as spfa on sfq.Ques_no=spfa.Ques_no where spfa.User_Id='" + id + "'", con);
                    con.Open();
                    List<FeedBackDetailDtl> questions = new List<FeedBackDetailDtl>();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        int i = 1;
                        
                        while (sdr.Read())
                        {
                            FeedBackDetailDtl PF = new FeedBackDetailDtl();
                            PF.Qid = i++;
                            PF.Qno = Convert.ToInt32(sdr["Quesno"]);
                            PF.Questiontxt = sdr["QuesText"].ToString();
                            PF.Answertxt = sdr["AnsText"].ToString();
                            questions.Add(PF);
                        }
                        return questions;
                    }
                    else if(sdr.HasRows==false)
                    {
                        FeedBackDetailDtl PF = new FeedBackDetailDtl();
                        PF.message = "Does not have feedback";
                        questions.Add(PF);
                        return questions;
                        //return null;
                    }
                    return questions;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<CommentsAndRatingDtl> GetComRatingFdbDtlById(int UId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {

                    SqlCommand cmd = new SqlCommand("select top 10 * from tblCommentsNRating where User_Id='" + UId + "'", con);
                    con.Open();
                    List<CommentsAndRatingDtl> li = new List<CommentsAndRatingDtl>();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        
                        while (sdr.Read())
                        {
                            CommentsAndRatingDtl cmt = new CommentsAndRatingDtl();
                            cmt.UId = Convert.ToInt32(sdr["User_Id"]);
                            cmt.UserTypeId = Convert.ToInt32(sdr["UserTypeId"]);
                            cmt.comment = sdr["Comment"].ToString();
                            cmt.rating = Convert.ToInt32(sdr["Rating"]);
                            li.Add(cmt);
                        }

                        return li;
                    }
                    else if(sdr.HasRows==false)
                    {
                        CommentsAndRatingDtl cmt = new CommentsAndRatingDtl();
                        cmt.message = "1";
                        li.Add(cmt);
                        //return li;   
                        //return null;
                    }
                    return li;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SPP_FBQuestionSet GetUserQuesAnsDtlById(List<FeedBackDetail> questions)
        {
            try
            {
                SPP_FBQuestionSet SQS = new SPP_FBQuestionSet();
                if (questions != null)
                {
                foreach (var ques in questions)
                {
                    switch (ques.Qid)
                    {
                        case 1:
                            {
                                SQS.qno1 = ques.Qno;
                                SQS.questext1 = ques.Questiontxt;
                                SQS.anstext1 = ques.Answertxt;
                                break;
                            }
                        case 2:
                            {
                                SQS.qno2 = ques.Qno;
                                SQS.questext2 = ques.Questiontxt;
                                SQS.anstext2 = ques.Answertxt;
                                break;
                            }
                        case 3:
                            {
                                SQS.qno3 = ques.Qno;
                                SQS.questext3 = ques.Questiontxt;
                                SQS.anstext3 = ques.Answertxt;
                                break;
                            }
                        case 4:
                            {
                                SQS.qno4 = ques.Qno;
                                SQS.questext4 = ques.Questiontxt;
                                SQS.anstext4 = ques.Answertxt;
                                break;
                            }
                        case 5:
                            {
                                SQS.qno5 = ques.Qno;
                                SQS.questext5 = ques.Questiontxt;
                                SQS.anstext5 = ques.Answertxt;
                                break;
                            }
                        case 6:
                            {
                                SQS.qno6 = ques.Qno;
                                SQS.questext6 = ques.Questiontxt;
                                SQS.anstext6 = ques.Answertxt;
                                break;
                            }
                    }
                }
                    return SQS;
                }
                else
                {
                    return SQS;
                }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SPP_FBQuestionSetDtl GetUserQuesAnsDtlById(List<FeedBackDetailDtl> questions)
        {
            try
            {
                SPP_FBQuestionSetDtl SQS = new SPP_FBQuestionSetDtl();
                if (questions != null)
                {
                    foreach (var ques in questions)
                    {
                        if (ques.message!=null)
                        {
                            SQS.message = ques.message;
                            SQS.message = "1";
                            return SQS;
                        }
                        switch (ques.Qid)
                        {
                            case 1:
                                {
                                    SQS.qno1 = ques.Qno;
                                    SQS.questext1 = ques.Questiontxt;
                                    SQS.anstext1 = ques.Answertxt;
                                    
                                    break;
                                }
                            case 2:
                                {
                                    SQS.qno2 = ques.Qno;
                                    SQS.questext2 = ques.Questiontxt;
                                    SQS.anstext2 = ques.Answertxt;
                                    
                                    break;
                                }
                            case 3:
                                {
                                    SQS.qno3 = ques.Qno;
                                    SQS.questext3 = ques.Questiontxt;
                                    SQS.anstext3 = ques.Answertxt;
                                    
                                    break;
                                }
                            case 4:
                                {
                                    SQS.qno4 = ques.Qno;
                                    SQS.questext4 = ques.Questiontxt;
                                    SQS.anstext4 = ques.Answertxt;
                                    
                                    break;
                                }
                            case 5:
                                {
                                    SQS.qno5 = ques.Qno;
                                    SQS.questext5 = ques.Questiontxt;
                                    SQS.anstext5 = ques.Answertxt;
                                    
                                    break;
                                }
                            case 6:
                                {
                                    SQS.qno6 = ques.Qno;
                                    SQS.questext6 = ques.Questiontxt;
                                    SQS.anstext6 = ques.Answertxt;
                                    
                                    break;
                                }
                        }
                    }
                     return SQS;
                }
                else
                {
                    return SQS;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<FeedBackDetail> GetParentFeedbackQuesAns(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_userfeedback", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "ParentFeedbackDtl");
                    cmd.Parameters.AddWithValue("@UserId", id);
                    List<FeedBackDetail> parentDtl = new List<FeedBackDetail>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        int i = 1;
                        while (sdr.Read())
                        {
                            FeedBackDetail PF = new FeedBackDetail();
                            PF.Qid = i++;

                            PF.ParentName = sdr["ParentName"].ToString();
                            PF.ContactNo = sdr["Contactno"].ToString();
                            PF.emailId = sdr["EmailId"].ToString();
                            PF.Qno = Convert.ToInt32(sdr["QuesNo"]);
                            PF.Questiontxt = sdr["Questions"].ToString();
                            PF.Answertxt = sdr["Answer"].ToString();
                            parentDtl.Add(PF);
                        }
                        return parentDtl;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<ParentFeedBackDetail> GetParentFeedbackQuesAnsDtl(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_userfeedback", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "ParentFeedbackDtl");
                    cmd.Parameters.AddWithValue("@UserId", id);
                    List<ParentFeedBackDetail> parentDtl = new List<ParentFeedBackDetail>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        int i = 1;
                        while (sdr.Read())
                        {
                            ParentFeedBackDetail PF = new ParentFeedBackDetail();
                            PF.Qid = i++;

                            PF.ParentName = sdr["ParentName"].ToString();
                            PF.ContactNo = sdr["Contactno"].ToString();
                            PF.emailId = sdr["EmailId"].ToString();
                            PF.Qno = Convert.ToInt32(sdr["QuesNo"]);
                            PF.Questiontxt = sdr["Questions"].ToString();
                            PF.Answertxt = sdr["Answer"].ToString();
                            parentDtl.Add(PF);
                        }
                        return parentDtl;
                    }
                    else if(sdr.HasRows==false)
                    {
                        ParentFeedBackDetail PF = new ParentFeedBackDetail();
                        PF.message = "Does have feedback";
                        parentDtl.Add(PF);
                        
                        //return null;
                    }
                    return parentDtl;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Parent_FBQuestionSetDtl GetParentFeedbackQuesAnsById(List<ParentFeedBackDetail> questions)
        {
            try
            {

                Parent_FBQuestionSetDtl SQS = new Parent_FBQuestionSetDtl();

                foreach (var ques in questions)
                {
                    if (ques.message != null)
                    {
                        SQS.message = ques.message;
                        SQS.message = "1";
                        return SQS;
                    }
                    switch (ques.Qid)
                    {
                       case 1:
                           {
                                SQS.ParentName = ques.ParentName;
                                SQS.ContactNo = ques.ContactNo;
                                SQS.emailId = ques.emailId;
                                SQS.qno1 = ques.Qno;
                                SQS.questext1 = ques.Questiontxt;
                                SQS.anstext1 = ques.Answertxt;
                                break;
                            }
                        case 2:
                            {
                                SQS.ParentName = ques.ParentName;
                                SQS.ContactNo = ques.ContactNo;
                                SQS.emailId = ques.emailId;
                                SQS.qno2 = ques.Qno;
                                SQS.questext2 = ques.Questiontxt;
                                SQS.anstext2 = ques.Answertxt;
                                break;
                            }
                        case 3:
                            {
                                SQS.ParentName = ques.ParentName;
                                SQS.ContactNo = ques.ContactNo;
                                SQS.emailId = ques.emailId;
                                SQS.qno3 = ques.Qno;
                                SQS.questext3 = ques.Questiontxt;
                                SQS.anstext3 = ques.Answertxt;
                                break;
                            }
                        case 4:
                            {
                                SQS.ParentName = ques.ParentName;
                                SQS.ContactNo = ques.ContactNo;
                                SQS.emailId = ques.emailId;
                                SQS.qno4 = ques.Qno;
                                SQS.questext4 = ques.Questiontxt;
                                SQS.anstext4 = ques.Answertxt;
                                break;
                            }
                        case 5:
                            {
                                SQS.ParentName = ques.ParentName;
                                SQS.ContactNo = ques.ContactNo;
                                SQS.emailId = ques.emailId;
                                SQS.qno5 = ques.Qno;
                                SQS.questext5 = ques.Questiontxt;
                                SQS.anstext5 = ques.Answertxt;
                                break;
                            }
                        case 6:
                            {
                                SQS.ParentName = ques.ParentName;
                                SQS.ContactNo = ques.ContactNo;
                                SQS.emailId = ques.emailId;
                                SQS.qno6 = ques.Qno;
                                SQS.questext6 = ques.Questiontxt;
                                SQS.anstext6 = ques.Answertxt;
                                break;
                            }
                    }
                }
                return SQS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SPP_FBQuestionSet GetParentFeedbackQuesAnsById(List<FeedBackDetail> questions)
        {
            try
            {

                SPP_FBQuestionSet SQS = new SPP_FBQuestionSet();

                foreach (var ques in questions)
                {
                    switch (ques.Qid)
                    {

                        case 1:
                            {
                                SQS.ParentName = ques.ParentName;
                                SQS.ContactNo = ques.ContactNo;
                                SQS.emailId = ques.emailId;
                                SQS.qno1 = ques.Qno;
                                SQS.questext1 = ques.Questiontxt;
                                SQS.anstext1 = ques.Answertxt;
                                break;
                            }
                        case 2:
                            {
                                SQS.ParentName = ques.ParentName;
                                SQS.ContactNo = ques.ContactNo;
                                SQS.emailId = ques.emailId;
                                SQS.qno2 = ques.Qno;
                                SQS.questext2 = ques.Questiontxt;
                                SQS.anstext2 = ques.Answertxt;
                                break;
                            }
                        case 3:
                            {
                                SQS.ParentName = ques.ParentName;
                                SQS.ContactNo = ques.ContactNo;
                                SQS.emailId = ques.emailId;
                                SQS.qno3 = ques.Qno;
                                SQS.questext3 = ques.Questiontxt;
                                SQS.anstext3 = ques.Answertxt;
                                break;
                            }
                        case 4:
                            {
                                SQS.ParentName = ques.ParentName;
                                SQS.ContactNo = ques.ContactNo;
                                SQS.emailId = ques.emailId;
                                SQS.qno4 = ques.Qno;
                                SQS.questext4 = ques.Questiontxt;
                                SQS.anstext4 = ques.Answertxt;
                                break;
                            }
                        case 5:
                            {
                                SQS.ParentName = ques.ParentName;
                                SQS.ContactNo = ques.ContactNo;
                                SQS.emailId = ques.emailId;
                                SQS.qno5 = ques.Qno;
                                SQS.questext5 = ques.Questiontxt;
                                SQS.anstext5 = ques.Answertxt;
                                break;
                            }
                        case 6:
                            {
                                SQS.ParentName = ques.ParentName;
                                SQS.ContactNo = ques.ContactNo;
                                SQS.emailId = ques.emailId;
                                SQS.qno6 = ques.Qno;
                                SQS.questext6 = ques.Questiontxt;
                                SQS.anstext6 = ques.Answertxt;
                                break;
                            }
                    }
                }
                return SQS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserDetail> GetShadowCDFListToAssignStudentByPincode(string pincode, int CandId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetShadowCDFToAssign");
                    cmd.Parameters.AddWithValue("@pinCode", pincode);
                    cmd.Parameters.AddWithValue("@StudentId", CandId);

                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            UserDetail UD = new UserDetail();
                            UD.ShadowCDFAvailableMsg = "1";// "CDF is Already Assigned to Candidate...";
                            li.Add(UD);
                        }
                        else
                        {
                            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                            {

                                UserDetail UD = new UserDetail();
                                UD.ShadowCDFAvailableMsg = "2";
                                UD.CDFId = Convert.ToInt32(ds.Tables[1].Rows[i]["uId"]);
                                UD.CDFName = ds.Tables[1].Rows[i]["CDFName"].ToString();
                                UD.CDFGender = ds.Tables[1].Rows[i]["gender"].ToString();
                                UD.CDFContact = ds.Tables[1].Rows[i]["contactNo"].ToString();
                                UD.CDFEmail = ds.Tables[1].Rows[i]["email"].ToString();
                                int CDFLevel = Convert.ToInt32(ds.Tables[1].Rows[i]["CDFLevel"]);
                                string CDFLvl = "L" + CDFLevel;
                                UD.CDFLevels = CDFLvl;
                                UD.CDFAddress = ds.Tables[1].Rows[i]["address"].ToString();
                                UD.CDFPinCode = ds.Tables[1].Rows[i]["pincode"].ToString();
                                UD.CDFCity = ds.Tables[1].Rows[i]["name"].ToString();
                                li.Add(UD);
                            }
                        }
                    }
                    return li;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserDetail> GetShadowCDFListToAssignStudentByEmailId(string emailId, int CandId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetShadowCDFToAssign");
                    cmd.Parameters.AddWithValue("@email", emailId);
                    cmd.Parameters.AddWithValue("@StudentId", CandId);

                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            UserDetail UD = new UserDetail();
                            UD.ShadowCDFAvailableMsg = "1";// "CDF is Already Assigned to Candidate...";
                            li.Add(UD);
                        }
                        else
                        {
                            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                            {

                                UserDetail UD = new UserDetail();
                                UD.ShadowCDFAvailableMsg = "2";
                                UD.CDFId = Convert.ToInt32(ds.Tables[1].Rows[i]["uId"]);
                                UD.CDFName = ds.Tables[1].Rows[i]["CDFName"].ToString();
                                UD.CDFGender = ds.Tables[1].Rows[i]["gender"].ToString();
                                UD.CDFContact = ds.Tables[1].Rows[i]["contactNo"].ToString();
                                UD.CDFEmail = ds.Tables[1].Rows[i]["email"].ToString();
                                int CDFLevel = Convert.ToInt32(ds.Tables[1].Rows[i]["CDFLevel"]);
                                string CDFLvl = "L" + CDFLevel;
                                UD.CDFLevels = CDFLvl;
                                UD.CDFAddress = ds.Tables[1].Rows[i]["address"].ToString();
                                UD.CDFPinCode = ds.Tables[1].Rows[i]["pincode"].ToString();
                                UD.CDFCity = ds.Tables[1].Rows[i]["name"].ToString();
                                li.Add(UD);
                            }
                        }
                    }
                    return li;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserDetail> GetShadowCDFListToAssignStudentByCityName(string City, int CandId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetShadowCDFToAssign");
                    cmd.Parameters.AddWithValue("@CityNm", City);
                    cmd.Parameters.AddWithValue("@StudentId", CandId);

                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            UserDetail UD = new UserDetail();
                            UD.ShadowCDFAvailableMsg = "1";// "CDF is Already Assigned to Candidate...";
                            li.Add(UD);
                        }
                        else
                        {
                            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                            {

                                UserDetail UD = new UserDetail();
                                UD.ShadowCDFAvailableMsg = "2";
                                UD.CDFId = Convert.ToInt32(ds.Tables[1].Rows[i]["uId"]);
                                UD.CDFName = ds.Tables[1].Rows[i]["CDFName"].ToString();
                                UD.CDFGender = ds.Tables[1].Rows[i]["gender"].ToString();
                                UD.CDFContact = ds.Tables[1].Rows[i]["contactNo"].ToString();
                                UD.CDFEmail = ds.Tables[1].Rows[i]["email"].ToString();
                                int CDFLevel = Convert.ToInt32(ds.Tables[1].Rows[i]["CDFLevel"]);
                                string CDFLvl = "L" + CDFLevel;
                                UD.CDFLevels = CDFLvl;
                                UD.CDFAddress = ds.Tables[1].Rows[i]["address"].ToString();
                                UD.CDFPinCode = ds.Tables[1].Rows[i]["pincode"].ToString();
                                UD.CDFCity = ds.Tables[1].Rows[i]["name"].ToString();
                                li.Add(UD);
                            }
                        }
                    }
                    return li;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool PostApproveGraph(string compCandID,int ApprovedBy)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue ("@GraphApprove",true);
                    cmd.Parameters.AddWithValue("@UserId", ApprovedBy);
                    cmd.Parameters.AddWithValue("@StudentId", compCandID);
                    cmd.Parameters.AddWithValue("@Type", "PostGraphApproveStatus");
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        return true;
                    }
                    else
                    {

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      

        //public  List<UserDetail> GetShadowCDFListToAssignStudent(string candPinCode, int candId)
        //{
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(strcon))
        //        {
        //            SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@Type", "GetCDFToAssign");
        //            cmd.Parameters.AddWithValue("@pinCode", lblStudentPinCode);
        //            cmd.Parameters.AddWithValue("@StudentId", 123);
        //            List<UserDetail> li = new List<UserDetail>();
        //            con.Open();

        //            SqlDataAdapter sda = new SqlDataAdapter(cmd);
        //            DataSet ds = new DataSet();
        //            sda.Fill(ds);
        //            if (ds != null)
        //            {
        //                if (ds.Tables[0].Rows.Count > 0)
        //                {
        //                    UserDetail UD = new UserDetail();
        //                    UD.CDFAvailableMsg = "1";// "CDF is Already Assigned to Candidate...";
        //                    li.Add(UD);
        //                }
        //                else
        //                {
        //                    // while(ds.Tables[1].Rows.Count > 0)
        //                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
        //                    {

        //                        UserDetail UD = new UserDetail();
        //                        UD.CDFAvailableMsg = "2";
        //                        UD.CDFId = Convert.ToInt32(ds.Tables[1].Rows[i]["uId"]);
        //                        UD.CDFName = ds.Tables[1].Rows[i]["CDFName"].ToString();
        //                        UD.CDFGender = ds.Tables[1].Rows[i]["gender"].ToString();
        //                        UD.CDFContact = ds.Tables[1].Rows[i]["contactNo"].ToString();
        //                        UD.CDFEmail = ds.Tables[1].Rows[i]["email"].ToString();
        //                        int CDFLevel = Convert.ToInt32(ds.Tables[1].Rows[i]["CDFLevel"]);
        //                        string CDFLvl = "L" + CDFLevel;
        //                        UD.CDFLevels = CDFLvl;
        //                        UD.CDFAddress = ds.Tables[1].Rows[i]["address"].ToString();
        //                        //UD.CDFPinCode = sdr["pincode"].ToString();     
        //                        UD.CDFPinCode = ds.Tables[1].Rows[i]["pincode"].ToString();
        //                        UD.CDFSesDate = ds.Tables[1].Rows[i]["CDFSesDate"].ToString();
        //                        li.Add(UD);
        //                    }
        //                }
        //            }
        //            return li;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public StudentTracking GetTestProdTrackingDtl(int studentId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentTrackingDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "TestProdDtl");
                    cmd.Parameters.AddWithValue("@Student_Id", studentId);
                    StudentTracking ST = new StudentTracking();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        sdr.Read();

                        DateTime TestDate = Convert.ToDateTime(sdr["TestCompDate"]);
                        if (TestDate == null)
                        {
                            ST.TestCompDate = "Test Not Completed Yet..";
                        }
                        else {

                            string TestD= sdr["TestCompDate"].ToString();
                            string[] TestCDate = TestD.Split(' ');

                            ST.TestCompDate = TestCDate[0];
                        }
                        
                        DateTime Product =Convert.ToDateTime(sdr["ProdPurchaseDate"]);
                        if (Product == null)
                        {
                            ST.ProdPurchDate = "Product Not Purchased Yet..";
                        }
                        else {

                            string ProdDate = sdr["ProdPurchaseDate"].ToString();
                            string[] ProductPDate = ProdDate.Split(' ');

                            ST.ProdPurchDate = ProductPDate[0];
                        }
                        



                    }
                    //else
                    //{
                    //    ST.TestCompDate = "Test Not Completed...";
                    //    ST.ProdPurchDate = "Product Not Purchased...";
                    //}
                    return ST;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public  StudentTracking GetSesCDFTrackingDtl(int StudentId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentTrackingDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "SesCDFDtl");
                    cmd.Parameters.AddWithValue("@Student_Id", StudentId);
                    StudentTracking ST = new StudentTracking();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        ST.SesStatus = sdr["SesStatus"].ToString();

                     //   string CDF = "CDF Assigned"+ sdr["CDFName"].ToString();
                        ST.CDFName = "Assigned..";

                    }
                    else
                    {
                        ST.SesStatus = "Not Assigned...";
                        ST.CDFName = "Not assigned...";
                    }
                    return ST;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserDetail> GetAsignCDFSessionDtlForCand()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "AssignedCDFAndSessionDtl");
                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            UserDetail UD = new UserDetail();
                                              
                            UD.Id = Convert.ToInt32(sdr["CandId"]);
                            UD.Name = sdr["CandName"].ToString();
                            UD.Gender = sdr["CandGender"].ToString();
                            UD.Contact = sdr["CandContact"].ToString();
                            UD.EmailId = sdr["CandEmail"].ToString();
                            UD.SessionId = Convert.ToInt32(sdr["SesId"]);
                            UD.City = sdr["Sessionvillage"].ToString();
                            UD.SessionDate = Convert.ToDateTime(sdr["SesDate"]);
                            UD.SesstionTime = sdr["SesTime"].ToString();
                            UD.Pincode = sdr["SesPin"].ToString();
                            UD.SesstionVenue = sdr["SesAddress"].ToString();
                            UD.CDFName = sdr["CDFName"].ToString();
                            UD.ShadowCDF = sdr["ShadowCDFName"].ToString();
                            //UD.ShadowDate = Convert.ToDateTime(sdr["ShadowDate"]);
                            UD.ShadowAssignDate = sdr["ShadowDate"].ToString();
                            if (UD.ShadowAssignDate == "1/1/2019")
                            {
                                UD.ShadowAssignDate = "NA";
                            }
                            else {
                                UD.ShadowAssignDate = sdr["ShadowDate"].ToString();
                            }
                            UD.CDFAcceptance = sdr["CDFAcceptance"].ToString();

                            UD.CDFGender = sdr["cdfGender"].ToString();
                            UD.CDFContact = sdr["cdfContact"].ToString();
                            UD.CDFEmail = sdr["cdfEmail"].ToString();
                            int levelCDF = Convert.ToInt32(sdr["cdfLevel"]);
                            string CDFLvl = "L" + levelCDF;
                            UD.CDFLevels = CDFLvl;

                            //bool Accept = Convert.ToBoolean(sdr["CDFAcceptance"]);
                            //if (Accept == true)
                            //{
                            //    UD.CDFAcceptance = "Yes";
                            //}
                            //else
                            //{
                            //    UD.CDFAcceptance = "No";
                            //}


                            li.Add(UD);
                        }
                    }
                    return li;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListItem> GetDDLSessionState()
        {
            try
            {
                List<SelectListItem> li = new List<SelectListItem>();
                using (SqlConnection con = new SqlConnection(strcon))
                {

                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetStateList");
                    cmd.Parameters.AddWithValue("@countryId", 101);
                   
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        li.Add(new SelectListItem
                        {
                            Text = dr["StateName"].ToString(),
                            Value = dr["StateId"].ToString()
                        });
                    }
                    con.Close();
                }
                return li; //
                           //  return new SelectList(li, "Value", "Text");
                           //return li;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

        public  List<SelectListItem> GetDDLSessionCity(int stateId)
        {
            try
            {
                List<SelectListItem> li = new List<SelectListItem>();
                using (SqlConnection con = new SqlConnection(strcon))
                {

                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetCityList");
                    cmd.Parameters.AddWithValue("@stateId", stateId);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        li.Add(new SelectListItem
                        {
                            Text = dr["CityName"].ToString(),
                            Value = dr["CityId"].ToString()
                        });
                    }
                    con.Close();
                }
                return li; //
                           //  return new SelectList(li, "Value", "Text");
                           //return li;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

        public int GetAsignCDFSessionForCandCount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    int AsgnCDFSes = 0;
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "AssignedCDFAndSenCnt");
                    UserDetail UD = new UserDetail();

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        AsgnCDFSes = Convert.ToInt32(sdr["ASignCDFSesCnt"]);

                    }

                    return AsgnCDFSes;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public  List<UserDetail> GetShadowCDFListToAssign(string candPinCode)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetAssignShadowCDF");
                    cmd.Parameters.AddWithValue("@pinCode", candPinCode);
                

                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            UserDetail UD = new UserDetail();


                            UD.CDFId = Convert.ToInt32(sdr["uId"]);
                            UD.CDFName = sdr["CDFName"].ToString();
                            UD.CDFGender = sdr["gender"].ToString();
                            UD.CDFContact = sdr["contactNo"].ToString();
                            UD.CDFEmail = sdr["email"].ToString();
                            int level= Convert.ToInt32(sdr["CDFLevel"]);
                            string CDFLevel = "L" + level;
                            UD.CDFLevels = CDFLevel;
                            UD.CDFAddress = sdr["address"].ToString();
                            UD.CDFPinCode = sdr["pincode"].ToString();


                            li.Add(UD);


                        }
                    }
                    return li;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public  bool InsertShadowCDFData(SessionDetails SD)
        {
            string shadowName = null;
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertSessionAndCDFDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Student_Id", SD.CandId);
                    cmd.Parameters.AddWithValue("@Shadow_Id", SD.ShadowCDF);
                    cmd.Parameters.AddWithValue("@Type", "InsertShadow");
                    con.Open();
                    
                    int i = cmd.ExecuteNonQuery();

                    string str = "select u.fname,s.Shadow_AssignDate from tbl_Session as s left join tblUserMaster as u on s.Shadow_CDFId=u.uId where s.Student_Id='" + SD.CandId + "'";
                    SqlCommand cmd1 = new SqlCommand(str, con);
                    SqlDataReader sdr = cmd1.ExecuteReader();
                    if (sdr.Read())
                    {
                        shadowName = sdr["fname"].ToString();
                        string GetShadowSessionDate = sdr["Shadow_AssignDate"].ToString();
                        string dt = GetShadowSessionDate.ToString();
                        string[] sesdt = dt.Split(' ');
                        SD.ShadowSessionDate = sesdt[0].ToString();
                    }


                    if (i >= 1)
                    {
                        //send Session Assign SMS To CDF
                        string SMSText = ConfigurationManager.AppSettings["assignShadowSessionSMS"].ToString();
                        SMSText = SMSText.Replace("{CDFName}", "" + shadowName);
                        SMSText = SMSText.Replace("{SessionDate}", "" + SD.ShadowSessionDate);
                        sendSms(SD.ShadowContact, SMSText);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserDetail> GetTestReassignedCandidateList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "TestReassignedCand");
                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            UserDetail UD = new UserDetail();
                            string FullName = "";
                            string Fname = sdr["fname"].ToString();
                            string Lname = sdr["lname"].ToString();
                            FullName = Fname + " " + Lname;
                            UD.Name = FullName;
                            UD.Id = Convert.ToInt32(sdr["Id"]);
                            UD.City = sdr["City"].ToString();
                            UD.DOB = Convert.ToDateTime(sdr["dob"]);
                            UD.Address = sdr["address"].ToString();
                            UD.RegDate = Convert.ToDateTime(sdr["regDateTime"]);
                            UD.Gender = sdr["gender"].ToString();
                            UD.Contact = sdr["contactNo"].ToString();
                            UD.EmailId = sdr["email"].ToString();
                            //UD.Pincode = sdr["pincode"].ToString();
                            // UD.UserStatus = sdr["userStatus"].ToString();
                            UD.TestStatus = sdr["testStatus"].ToString();
                            // UD.TestCompDate = Convert.ToDateTime(sdr["testCopleteDate"]);
                            li.Add(UD);


                        }
                    }
                    return li;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetCommentAndRatingCount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    int commentandRating = 0;
                    SqlCommand cmd = new SqlCommand("sp_GetCommentAndRating", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "RatingAndCommentCount");
                    UserDetail UD = new UserDetail();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        commentandRating = Convert.ToInt32(sdr["commentandRating"]);

                    }

                    return commentandRating;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<CommnetAndRating> GetCommentAndRating()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetCommentAndRating", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "commnetAndRating");
                    List<CommnetAndRating> cmtList = new List<CommnetAndRating>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            CommnetAndRating CR = new CommnetAndRating();
                            CR.Id = Convert.ToInt32(sdr["Id"]);
                            CR.UserTypeId = Convert.ToInt32(sdr["UserTypeId"]);
                            CR.userTypeName = sdr["userTypeName"].ToString();
                            CR.Name = sdr["UserName"].ToString();
                            //CR.fname = sdr["fname"].ToString();
                            //CR.lname = sdr["lname"].ToString();
                            CR.gender = sdr["gender"].ToString();
                            // CR.dob = Convert.ToDateTime(sdr["dob"]);
                            //string Birth = sdr["dob"].ToString();
                            //string[] birthdate = Birth.Split(' ');
                            //CR.BirthDate = birthdate[0].ToString();
                            CR.contactNo = sdr["contactNo"].ToString();
                            CR.email = sdr["email"].ToString();
                            string cmt = sdr["CommentDate"].ToString();
                            string[] cmtdt = cmt.Split(' ');
                            CR.CommentDate = cmtdt[0].ToString();
                            CR.Comment = sdr["Comment"].ToString();
                            CR.Rating = Convert.ToInt32(sdr["Rating"]);
                            cmtList.Add(CR);
                        }
                    }
                    return cmtList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }






        public int GetTestReassignedCandCount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    int TestReassignedCount = 0;
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "TestReassignedCandCount");
                    UserDetail UD = new UserDetail();

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        TestReassignedCount = Convert.ToInt32(sdr["TestReassignCnt"]);

                    }

                    return TestReassignedCount;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<UserDetail> GetTestCompletedCandidateList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "TestCompletedCand");
                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            UserDetail UD = new UserDetail();
                            string FullName = "";
                            string Fname = sdr["fname"].ToString();
                            string Lname = sdr["lname"].ToString();
                            FullName = Fname + " " + Lname;
                            UD.Name = FullName;
                            UD.Id = Convert.ToInt32(sdr["Id"]);
                            UD.City = sdr["City"].ToString();
                            UD.DOB = Convert.ToDateTime(sdr["dob"]);
                            UD.Address = sdr["address"].ToString();
                            UD.RegDate = Convert.ToDateTime(sdr["regDateTime"]);
                            UD.Gender = sdr["gender"].ToString();
                            UD.Contact = sdr["contactNo"].ToString();
                            UD.EmailId = sdr["email"].ToString();
                            UD.Pincode = sdr["pincode"].ToString();
                            // UD.UserStatus = sdr["userStatus"].ToString();
                            UD.TestStatus = sdr["testStatus"].ToString();
                            UD.TestCompDate = Convert.ToDateTime(sdr["testCopleteDate"]);
                            UD.GraphApproved = sdr["GraphApproved"].ToString();
                            li.Add(UD);


                        }
                    }
                    return li;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetTestCompletedCandCount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    int TestCompleteCount = 0;
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "TestCompletedCandCount");
                    UserDetail UD = new UserDetail();

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        TestCompleteCount = Convert.ToInt32(sdr["TestCompCnt"]);

                    }

                    return TestCompleteCount;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<UserDetail> GetCDFListToAssignStudent(string lblStudentPinCode, int CandId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetCDFToAssign");
                    cmd.Parameters.AddWithValue("@pinCode", lblStudentPinCode);
                    cmd.Parameters.AddWithValue("@StudentId", CandId);
                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            UserDetail UD = new UserDetail();
                            UD.CDFAvailableMsg = "1";// "CDF is Already Assigned to Candidate...";
                            li.Add(UD);
                        }
                        else
                        {
                           // while(ds.Tables[1].Rows.Count > 0)
                           for(int i=0; i< ds.Tables[1].Rows.Count;i++ )
                            {

                                UserDetail UD = new UserDetail();
                                UD.CDFAvailableMsg = "2";
                                UD.CDFId = Convert.ToInt32(ds.Tables[1].Rows[i]["uId"]);
                                UD.CDFName = ds.Tables[1].Rows[i]["CDFName"].ToString();
                                UD.CDFGender = ds.Tables[1].Rows[i]["gender"].ToString();
                                UD.CDFContact = ds.Tables[1].Rows[i]["contactNo"].ToString();
                                UD.CDFEmail = ds.Tables[1].Rows[i]["email"].ToString();
                                int CDFLevel = Convert.ToInt32(ds.Tables[1].Rows[i]["CDFLevel"]);
                                string CDFLvl = "L" + CDFLevel;
                                UD.CDFLevels = CDFLvl;
                                UD.CDFAddress = ds.Tables[1].Rows[i]["address"].ToString();
                                //UD.CDFPinCode = sdr["pincode"].ToString();     
                                UD.CDFPinCode = ds.Tables[1].Rows[i]["pincode"].ToString();
                                UD.CDFSesDate = ds.Tables[1].Rows[i]["CDFSesDate"].ToString();
                                UD.CDFSessionCnt = Convert.ToInt32(ds.Tables[1].Rows[i]["SessionCount"]);
                                li.Add(UD);
                            }
                        }


                    }

                    //SqlDataReader sdr = cmd.ExecuteReader();
                    //if (sdr.HasRows)
                    //{
                    //    while (sdr.Read())
                    //    {
                    //        UserDetail UD = new UserDetail();


                    //        UD.CDFId = Convert.ToInt32(sdr["uId"]);
                    //        UD.CDFName = sdr["CDFName"].ToString();
                    //        UD.CDFGender = sdr["gender"].ToString();
                    //        UD.CDFContact = sdr["contactNo"].ToString();
                    //        UD.CDFEmail = sdr["email"].ToString();
                    //        int CDFLevel = Convert.ToInt32(sdr["CDFLevel"]);
                    //        string CDFLvl = "L" + CDFLevel;
                    //        UD.CDFLevels = CDFLvl;
                    //        UD.CDFAddress = sdr["address"].ToString();
                    //        //UD.CDFPinCode = sdr["pincode"].ToString();     
                    //        UD.CDFPinCode = sdr["pincode"].ToString();


                    //        li.Add(UD);


                    //    }
                    //}
                    return li;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserDetail> GetCDFListToAssign(string lblStudentPinCode)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetCDFToAssign");    
                    cmd.Parameters.AddWithValue("@pinCode", lblStudentPinCode);
                   
                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();


                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            UserDetail UD = new UserDetail();


                            UD.CDFId = Convert.ToInt32(sdr["uId"]);
                            UD.CDFName = sdr["CDFName"].ToString();
                            UD.CDFGender = sdr["gender"].ToString();
                            UD.CDFContact = sdr["contactNo"].ToString();
                            UD.CDFEmail = sdr["email"].ToString();
                            int CDFLevel = Convert.ToInt32(sdr["CDFLevel"]);
                            string CDFLvl = "L" + CDFLevel;
                            UD.CDFLevels = CDFLvl;
                            UD.CDFAddress = sdr["address"].ToString();
                            //UD.CDFPinCode = sdr["pincode"].ToString();     
                            UD.CDFPinCode = sdr["pincode"].ToString();


                            li.Add(UD);


                        }
                    }
                    return li;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool InsertSessionCDFData(SessionDetails SD)
        {
            string cdfName = null;
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertSessionAndCDFDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CDF_Id", SD.CDFId);
                    cmd.Parameters.AddWithValue("@Student_Id", SD.CandId);
                    cmd.Parameters.AddWithValue("@Session_Date", SD.SesDate);
                    //cmd.Parameters.AddWithValue("@Session_Time", SD.SessionTime);
                    cmd.Parameters.AddWithValue("@Session_Time", SD.FinalTime);
                    cmd.Parameters.AddWithValue("@CreatedBy", SD.LoginClientId);
                   // cmd.Parameters.AddWithValue("@pin", SD.sessionPinCode);
                    cmd.Parameters.AddWithValue("@Address", SD.sessionVenue);
                    cmd.Parameters.AddWithValue("@cityId", SD.AreaId);
                    cmd.Parameters.AddWithValue("@Type", "InsertCDF");
                    cmd.Parameters.AddWithValue("@UpdatedBy", SD.LoginClientId);
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    string str = "select u.fname from tbl_Session as s left join tblUserMaster as u on s.CDF_Id=u.uId where s.Student_Id='" + SD.CandId + "'";
                    SqlCommand cmd1 = new SqlCommand(str, con);
                    SqlDataReader sdr = cmd1.ExecuteReader();
                    if (sdr.Read())
                    {
                        cdfName = sdr["fname"].ToString();
                    }

                    if (i >= 1)
                    {
                       
                            //send Session Assign SMS To CDF
                        string SMSText = ConfigurationManager.AppSettings["assignSessionSMS"].ToString();
                        SMSText = SMSText.Replace("{CDFName}", "" + cdfName);
                        SMSText = SMSText.Replace("{SessionDate}", "" + SD.SesDate);
                        //SMSText = SMSText.Replace("{SessionTime}", "" + SD.SessionTime);
                        sendSms(SD.CDFContact, SMSText);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //Send SMS Function
        public Boolean sendSms(string mob, string msg)
        {
            string result = "";
            WebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                string userid = ConfigurationManager.AppSettings["SMSUserId"].ToString();  //  "2000167436";
                string passwd = ConfigurationManager.AppSettings["SMSPassword"].ToString();  //  "xzreMXXv5";
                string url =
            "http://enterprise.smsgupshup.com/GatewayAPI/rest?method=sendMessage&send_to=" +
            mob + "&msg=" + msg + "&userid=" + userid + "&password=" + passwd + "&v=1.1&msg_type=TEXT&auth_scheme=PLAIN";

                request = WebRequest.Create(url);
                response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding ec = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader reader = new System.IO.StreamReader(stream, ec);
                result = reader.ReadToEnd();
                reader.Close();
                stream.Close();
                return true;
            }
            catch (Exception ex)
            {
               // Log.Error("" + ex);
                return false;
            }
        }

        public List<SelectListItem> GetDDLSessionArea(string pincode)
        {
            try
            {
                List<SelectListItem> li = new List<SelectListItem>();
                using (SqlConnection con = new SqlConnection(strcon))
                {

                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetVillage");
                    cmd.Parameters.AddWithValue("@pinCode", pincode);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        li.Add(new SelectListItem
                        {
                            Text = dr["VillageName"].ToString(),
                            Value = dr["VillageId"].ToString()
                        });
                    }
                    con.Close();
                }
                 return li; //
              //  return new SelectList(li, "Value", "Text");
                //return li;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

        public List<UserDetail> GetStudentDeailsByID(int candID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "StudentDetailsById");
                    cmd.Parameters.AddWithValue("@id", candID);
                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();

                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {

                            UserDetail UD = new UserDetail();
                            UD.Id = Convert.ToInt32(sdr["Id"]);
                            UD.Fname = sdr["fname"].ToString();
                            UD.Lname = sdr["lname"].ToString();
                            UD.Gender = sdr["gender"].ToString();
                            UD.DOB = Convert.ToDateTime(sdr["dob"]);
                            UD.Pincode = sdr["pincode"].ToString();
                            UD.Address = sdr["address"].ToString();
                            UD.City = sdr["village"].ToString();
                            li.Add(UD);
                        }
                    }
                    return li;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SessionDetailsModel> GetSessionDetailsByID(int Uid)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetSessionDtl");
                    cmd.Parameters.AddWithValue("@id", Uid);
                    List<SessionDetailsModel> li = new List<SessionDetailsModel>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            SessionDetailsModel UD = new SessionDetailsModel();
                            //Student Details
                            UD.Id = Convert.ToInt32(sdr["stdid"]);
                            UD.FullName = sdr["StdName"].ToString();
                            UD.Gender = sdr["stdGender"].ToString();
                            UD.DOB = Convert.ToDateTime(sdr["stddob"]);
                            UD.Pincode = sdr["pincode"].ToString();
                            UD.Address = sdr["address"].ToString();
                            UD.City = sdr["village"].ToString();

                            //CDF Details
                            int cdfid = 0;
                            UD.Id = Convert.ToInt32(sdr["stdid"]);
                            //UD.CDFId = Convert.ToInt32(sdr["CDFId"]);
                            if (sdr["CDFId"] != System.DBNull.Value)
                            {
                                cdfid = Convert.ToInt32(sdr["CDFId"]);
                            }
                            else
                            {
                                cdfid = 0;
                            }
                            UD.CDFId = cdfid;
                            UD.CDFName = sdr["CDFName"].ToString();
                            UD.CDFGender = sdr["CDFGender"].ToString();
                            UD.CDFContact = sdr["CDFContact"].ToString();
                            UD.CDFEmail = sdr["CDFEmail"].ToString();
                            //int Level = Convert.ToInt32(sdr["CDFLevel"]);
                            int Level = 0;
                            if (sdr["CDFLevel"] != System.DBNull.Value)
                            {
                                Level= Convert.ToInt32(sdr["CDFLevel"]);
                            }
                            else
                            {
                                Level = 0;
                            }
                            string strLevel = "L" + Level;
                            UD.CDFLevels = strLevel;

                            //Shadow Details
                            int shadowid = 0;
                            //UD.ShadowId = Convert.ToInt32(sdr["ShadowId"]);
                            if (sdr["ShadowId"] != System.DBNull.Value)
                            {
                                shadowid = Convert.ToInt32(sdr["ShadowId"]);
                            }
                            else
                            {
                                shadowid = 0;
                            }
                            UD.ShadowId = shadowid;
                            UD.ShadowName = sdr["ShadowName"].ToString();
                            UD.ShadowGender = sdr["ShadowGender"].ToString();
                            UD.ShadowContact = sdr["ShadowContact"].ToString();
                            UD.ShadowEmail = sdr["ShadowEmail"].ToString();
                            //int Levels = Convert.ToInt32(sdr["ShadowLevel"]);
                            int Levels = 0;
                            if (sdr["ShadowLevel"] != System.DBNull.Value)
                            {
                                Levels = Convert.ToInt32(sdr["ShadowLevel"]);
                            }
                            else
                            {
                                Levels = 0;
                            }
                            string strLevels = "L" + Levels;
                            UD.ShadowLevel = strLevels;

                            //Session Details
                            int sesid = 0;
                            //UD.SessionId = Convert.ToInt32(sdr["SesId"]);
                            if (sdr["SesId"] != System.DBNull.Value)
                            {
                                sesid = Convert.ToInt32(sdr["SesId"]);
                            }
                            else
                            {
                                sesid = 0;
                            }
                            UD.SessionId = sesid;
                            UD.SessionDate = Convert.ToDateTime(sdr["SesDate"]);
                            UD.SesstionTime = sdr["SesTime"].ToString();
                            UD.SessionStatus = sdr["SesStatus"].ToString();
                            UD.SesCDFResponce = sdr["SesCDFResponce"].ToString();
                            UD.SesAddress = sdr["SesAddress"].ToString();
                            li.Add(UD);
                        }
                    }
                    return li;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public List<SessionDetailsModel> GetStudentDtlByID(int Uid)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Type", "StudentDetailsById");
                    cmd.Parameters.AddWithValue("@Type", "GetSessionDtl");
                    cmd.Parameters.AddWithValue("@id", Uid);
                    List<SessionDetailsModel> li = new List<SessionDetailsModel>();
                    con.Open();
                    //SessionDetailsModel UD = new SessionDetailsModel();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {

                            SessionDetailsModel UD = new SessionDetailsModel();
                            UD.Id = Convert.ToInt32(sdr["stdid"]);
                            //string fn = sdr["fname"].ToString();
                            //string ln = sdr["lname"].ToString();
                            //UD.FullName = fn + " " + ln;
                            UD.FullName = sdr["StdName"].ToString();
                            UD.Gender = sdr["stdGender"].ToString();
                            UD.DOB = Convert.ToDateTime(sdr["stddob"]);
                            UD.Pincode = sdr["pincode"].ToString();
                            UD.Address = sdr["address"].ToString();
                            UD.City = sdr["village"].ToString();
                            li.Add(UD);
                            
                        }
                    }
                     return li;
                    //return UD;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetSessionCmpltStudCount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    int SessionCompleteCount = 0;
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "SessionCompleteDtlCount");
                    UserDetail UD = new UserDetail();

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        SessionCompleteCount = Convert.ToInt32(sdr["SessCmpltCount"]);

                    }

                    return SessionCompleteCount;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserDetail> GetStudentSessionDeails(int candID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "CandSessionDtl");
                    cmd.Parameters.AddWithValue("@id", candID);
                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            UserDetail UD = new UserDetail();

                            //UD.Id = Convert.ToInt32(sdr["CanId"]);
                            //UD.Name = sdr["CandName"].ToString();
                            //UD.Contact = sdr["CandContact"].ToString();
                            //UD.EmailId = sdr["CandEmail"].ToString();
                            //UD.Address = sdr["CandAddress"].ToString();

                            //UD.CDFId = Convert.ToInt32(sdr["CDFId"]);
                            //UD.CDFName = sdr["CDFName"].ToString();
                            //UD.CDFGender = sdr["CDFGender"].ToString();
                            //UD.CDFContact = sdr["CDFContact"].ToString();
                            //UD.CDFEmail = sdr["CDFEmail"].ToString();
                            //UD.CDFLevel = Convert.ToInt32(sdr["CDFLevel"]);

                            UD.SessionId = Convert.ToInt32(sdr["SesId"]);
                            UD.SessionDate = Convert.ToDateTime(sdr["SesDate"]);
                            UD.SesstionTime = sdr["SesTime"].ToString();
                            UD.SessionStatus = sdr["SesStatus"].ToString();
                            UD.SesCDFResponce = sdr["SesCDFResponce"].ToString();
                            UD.SesAddress = sdr["SesAddress"].ToString();


                            li.Add(UD);


                        }
                    }
                    return li;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SessionDetailsModel> GetStudentSessionDeailsById(int candID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetSessionDtl");
                    cmd.Parameters.AddWithValue("@id", candID);
                    List<SessionDetailsModel> li = new List<SessionDetailsModel>();
                    con.Open();
                    //SessionDetailsModel UD = new SessionDetailsModel();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            //UserDetail UD = new UserDetail();
                            SessionDetailsModel UD = new SessionDetailsModel();
                            UD.SessionId = Convert.ToInt32(sdr["SesId"]);
                            UD.SessionDate = Convert.ToDateTime(sdr["SesDate"]);
                            UD.SesstionTime = sdr["SesTime"].ToString();
                            UD.SessionStatus = sdr["SesStatus"].ToString();
                            UD.SesCDFResponce = sdr["SesCDFResponce"].ToString();
                            UD.SesAddress = sdr["SesAddress"].ToString();
                            li.Add(UD);
                          }
                    }
                    return li;
                  }
              }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserDetail> GetStudentCDFDeails(int candID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "CandCDFDetails");
                    cmd.Parameters.AddWithValue("@id", candID);
                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            UserDetail UD = new UserDetail();

                            UD.Id = Convert.ToInt32(sdr["CanId"]);
                            //UD.Name = sdr["CandName"].ToString();
                            //UD.Contact = sdr["CandContact"].ToString();
                            //UD.EmailId = sdr["CandEmail"].ToString();
                            //UD.Address = sdr["CandAddress"].ToString();

                            UD.CDFId = Convert.ToInt32(sdr["CDFId"]);
                            UD.CDFName = sdr["CDFName"].ToString();
                            UD.CDFGender = sdr["CDFGender"].ToString();
                            UD.CDFContact = sdr["CDFContact"].ToString();
                            UD.CDFEmail = sdr["CDFEmail"].ToString();

                            int Level = Convert.ToInt32(sdr["CDFLevel"]);
                            string strLevel = "L" + Level;

                            UD.CDFLevels = strLevel;

                            //UD.SessionId = Convert.ToInt32(sdr["SesId"]);
                            //UD.SessionDate = Convert.ToDateTime(sdr["SesDate"]);
                            //UD.SessionTime = (TimeSpan)sdr["SesTime"];
                            //UD.SessionStatus = sdr["SesStatus"].ToString();
                            //UD.SesCDFResponce = sdr["SesCDFResponce"].ToString();
                            //UD.SesAddress = sdr["SesAddress"].ToString();


                            li.Add(UD);


                        }
                    }
                    return li;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SessionDetailsModel> GetStudentCDFDeailsById(int candID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetSessionDtl");
                    cmd.Parameters.AddWithValue("@id", candID);
                    List<SessionDetailsModel> li = new List<SessionDetailsModel>();
                    con.Open();
                    //SessionDetailsModel UD = new SessionDetailsModel();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            SessionDetailsModel UD = new SessionDetailsModel();
                            UD.Id = Convert.ToInt32(sdr["stdid"]);
                            UD.CDFId = Convert.ToInt32(sdr["CDFId"]);
                            UD.CDFName = sdr["CDFName"].ToString();
                            UD.CDFGender = sdr["CDFGender"].ToString();
                            UD.CDFContact = sdr["CDFContact"].ToString();
                            UD.CDFEmail = sdr["CDFEmail"].ToString();

                            int Level = Convert.ToInt32(sdr["CDFLevel"]);
                            string strLevel = "L" + Level;

                            UD.CDFLevels = strLevel;
                            li.Add(UD);
                        }
                    }
                    //return UD;
                    return li;

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserDetail> GetStudentShadowDetails(int candID)
        {
            try
            {
                using(SqlConnection con=new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList",con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "CandShadowDetails");
                    cmd.Parameters.AddWithValue("@id", candID);
                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            UserDetail UD = new UserDetail();
                            UD.Id = Convert.ToInt32(dr["CanId"]);
                            UD.ShadowId = Convert.ToInt32(dr["ShadowId"]);
                            UD.ShadowName = dr["ShadowName"].ToString();
                            UD.ShadowGender = dr["ShadowGender"].ToString();
                            UD.ShadowContact = dr["ShadowContact"].ToString();
                            UD.ShadowEmail = dr["ShadowEmail"].ToString();
                            int Level = Convert.ToInt32(dr["ShadowLevel"]);
                            string strLevel = "L" + Level;
                            UD.ShadowLevel = strLevel;
                            li.Add(UD);
                        }
                    }
                    return li;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<SessionDetailsModel> GetStudentShadowDetailsById(int candID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "GetSessionDtl");
                    cmd.Parameters.AddWithValue("@id", candID);
                    List<SessionDetailsModel> li = new List<SessionDetailsModel>();
                    con.Open();
                    //SessionDetailsModel UD = new SessionDetailsModel();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SessionDetailsModel UD = new SessionDetailsModel();
                            UD.Id = Convert.ToInt32(dr["stdid"]);
                            UD.ShadowId = Convert.ToInt32(dr["ShadowId"]);
                            UD.ShadowName = dr["ShadowName"].ToString();
                            UD.ShadowGender = dr["ShadowGender"].ToString();
                            UD.ShadowContact = dr["ShadowContact"].ToString();
                            UD.ShadowEmail = dr["ShadowEmail"].ToString();
                            int Level = Convert.ToInt32(dr["ShadowLevel"]);
                            string strLevel = "L" + Level;
                            UD.ShadowLevel = strLevel;
                            li.Add(UD);
                        }
                    }
                    else if (dr.HasRows == false)
                    {
                        SessionDetailsModel UD = new SessionDetailsModel();
                        UD.message = "Shadow CDF is not assign.";
                        li.Add(UD);
                    }
                    return li;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserDetail> GetSessionCompleteDetails()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "SessionCompleteDtl");
                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            UserDetail UD = new UserDetail();

                            UD.Id = Convert.ToInt32(sdr["CanId"]);
                            UD.Name = sdr["CandName"].ToString();
                            UD.Contact = sdr["CandContact"].ToString();
                            UD.EmailId = sdr["CandEmail"].ToString();
                            UD.Address = sdr["CandAddress"].ToString();

                            UD.CDFId = Convert.ToInt32(sdr["CDFId"]);
                            UD.CDFName = sdr["CDFName"].ToString();
                            UD.CDFGender = sdr["CDFGender"].ToString();
                            UD.CDFContact = sdr["CDFContact"].ToString();
                            UD.CDFEmail = sdr["CDFEmail"].ToString();
                            UD.CDFLevel = Convert.ToInt32(sdr["CDFLevel"]);

                            UD.SessionId = Convert.ToInt32(sdr["SesId"]);
                            UD.SessionDate = Convert.ToDateTime(sdr["SesDate"]);
                            // UD.SessionTime = (TimeSpan)sdr["SesTime"];
                            UD.SesstionTime= sdr["SesTime"].ToString();
                            //UD.SessionTime = (TimeSpan)sdr["SesTime"];
                            UD.SessionStatus = sdr["SesStatus"].ToString();
                            UD.SesCDFResponce = sdr["SesCDFResponce"].ToString();
                            UD.SesAddress = sdr["SesAddress"].ToString();

                            li.Add(UD);


                        }
                    }
                    return li;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetProdPurchaseStudCount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    int ProdPurchaseCount = 0;
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "ProductPurchaseStudCount");
                    UserDetail UD = new UserDetail();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        ProdPurchaseCount = Convert.ToInt32(sdr["ProdPurchaseStudCount"]);

                    }

                    return ProdPurchaseCount;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int authcodeCount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    int authcodeCount = 0;
                    SqlCommand cmd = new SqlCommand("sp_Authcode", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "authcodeCount");
                    Authcode auth = new Authcode();

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        authcodeCount = Convert.ToInt32(sdr["authcount"]);

                    }

                    return authcodeCount;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public List<Authcode> GetAuthcodePrivew()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_Authcode", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "AuthcodeList");
                    List<Authcode> li = new List<Authcode>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            Authcode auth = new Authcode();

                            // auth.promotorId=Convert.ToInt32(sdr["promotorId"]);
                            auth.id = Convert.ToInt32(sdr["id"]);
                            auth.prodName = sdr["prodName"].ToString();
                            auth.name = sdr["name"].ToString();
                            auth.date = Convert.ToDateTime(sdr["date"]);
                            auth.createdBy = Convert.ToInt32(sdr["createdBy"]);
                            auth.count = Convert.ToInt32(sdr["count"]);
                            auth.comment = sdr["comment"].ToString();
                            auth.adminUser = sdr["AdminUser"].ToString();
                            //  auth.status = sdr["status"].ToString();
                            //  auth.authcode = sdr["authcode"].ToString();                           
                            //   auth.validity = Convert.ToInt32(sdr["validity"]);
                            li.Add(auth);

                        }
                    }
                    return li;

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<Authcode> GetAuthcodePrivewById(int promotorId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {

                    SqlCommand cmd = new SqlCommand("sp_Authcode", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "AuthcodeList2");
                    cmd.Parameters.AddWithValue("@promotorId", promotorId);
                    List<Authcode> li = new List<Authcode>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            Authcode au = new Authcode();
                            au.promotorId = Convert.ToInt32(sdr["promotorId"]);
                            au.authcode = sdr["authcode"].ToString();
                            au.date = Convert.ToDateTime(sdr["date"]);
                            au.status = sdr["status"].ToString();
                            au.validity = Convert.ToInt32(sdr["validity"]);
                            au.EmailId = sdr["CandEmail"].ToString();
                            li.Add(au);
                        }
                    }
                    return li;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }









        public int GetRegistredStudentCount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    int RegStudentCount = 0;
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "TotStudCount");
                    UserDetail UD = new UserDetail();

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        RegStudentCount = Convert.ToInt32(sdr["RegStudCount"]);
                    }

                    return RegStudentCount;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public List<UserDetail> GetRegStudentList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "StudentList");
                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            UserDetail UD = new UserDetail();
                            string FullName = "";
                            string Fname = sdr["fname"].ToString();
                            string Lname = sdr["lname"].ToString();
                            FullName = Fname + " " + Lname;
                            UD.Name = FullName;
                            UD.Id = Convert.ToInt32(sdr["Id"]);
                            UD.City = sdr["City"].ToString();
                            //  UD.DOB = Convert.ToDateTime(sdr["dob"]);
                            string Birth = sdr["dob"].ToString();
                            string[] birthdate = Birth.Split(' ');
                            UD.BirthDate = birthdate[0].ToString();
                            UD.Address = sdr["address"].ToString();
                            UD.RegDate = Convert.ToDateTime(sdr["regDateTime"]);
                            UD.Gender = sdr["gender"].ToString();
                            UD.Contact = sdr["contactNo"].ToString();
                            UD.EmailId = sdr["email"].ToString();
                            UD.Pincode = sdr["pincode"].ToString();
                            UD.UserStatus = sdr["userStatus"].ToString();
                            UD.education = sdr["standard"].ToString();
                            li.Add(UD);


                        }
                    }
                    return li;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public List<UserDetail> GetProductPurchaseStudentList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "ProductPurchaseStudList");
                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            UserDetail UD = new UserDetail();
                            string FullName = "";
                            string Fname = sdr["fname"].ToString();
                            string Lname = sdr["lname"].ToString();
                            FullName = Fname + " " + Lname;
                            UD.Name = FullName;
                            UD.Id = Convert.ToInt32(sdr["Id"]);
                            UD.City = sdr["City"].ToString();
                            UD.DOB = Convert.ToDateTime(sdr["dob"]);
                            UD.Address = sdr["address"].ToString();
                            UD.RegDate = Convert.ToDateTime(sdr["regDateTime"]);
                            UD.Gender = sdr["gender"].ToString();
                            UD.Contact = sdr["contactNo"].ToString();
                            UD.EmailId = sdr["email"].ToString();
                            UD.Pincode = sdr["pincode"].ToString();
                            UD.UserStatus = sdr["userStatus"].ToString();
                            UD.TestStatus = sdr["CandTestStatus"].ToString();
                            UD.ProductName = sdr["prodName"].ToString();
                            int Price = Convert.ToInt32(sdr["price"]);
                            string ProdPrice= "Rs "+ Price + "/-";
                            UD.ProPrice1 = ProdPrice;
                            li.Add(UD);


                        }
                    }
                    return li;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //public List<CDFDetails> GetCDFDetails(int StudentId)
        //{
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(strcon))
        //        {
        //            SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@Type", "CDFList");
        //            cmd.Parameters.AddWithValue("@id", StudentId);
        //            List<CDFDetails> li = new List<CDFDetails>();
        //            con.Open();
        //            SqlDataReader sdr = cmd.ExecuteReader();
        //            if (sdr.HasRows)
        //            {
        //                while (sdr.Read())
        //                {
        //                    CDFDetails UD = new CDFDetails();
        //                    UD.CDFId = Convert.ToInt32(sdr["uId"]);
        //                    string FName = "", LName = "", Name = "";
        //                    FName = sdr["fname"].ToString();
        //                    LName = sdr["lname"].ToString();
        //                    Name = FName + " " + LName;
        //                    UD.CDFName = Name;
        //                    UD.CDFEmail = sdr["email"].ToString();
        //                    UD.CDFContact = sdr["contactNo"].ToString();
        //                    UD.CDFCity = sdr["name"].ToString();
        //                    li.Add(UD);
        //                }
        //            }
        //            return li;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        public UserDetail GetStudentMoreInfo(int studentId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "StudentDetailsById");
                    cmd.Parameters.AddWithValue("@id", studentId);
                    //  List<UserDetail> li = new List<UserDetail>();
                    con.Open();
                    UserDetail UD = new UserDetail();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            UD.Fname = sdr["fname"].ToString();
                            UD.Lname = sdr["lname"].ToString();
                            UD.Gender = sdr["gender"].ToString();
                            UD.Contact = sdr["contactNo"].ToString();
                            UD.EmailId = sdr["email"].ToString();
                            UD.Pincode = sdr["pincode"].ToString();
                            UD.Address = sdr["address"].ToString();
                            UD.Status = sdr["userStatus"].ToString();
                            //  li.Add(UD);
                        }
                    }
                    return UD;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<UserDetail> GetAllStudentList()
        {
            try
            {
                //using (SqlConnection con = new SqlConnection(strcon))
                //{
                //    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    cmd.Parameters.AddWithValue("@Type", "StudentList");
                //    List<UserDetail> li = new List<UserDetail>();
                //    con.Open();
                //    SqlDataReader sdr = cmd.ExecuteReader();
                //    if (sdr.HasRows)
                //    {
                //        while (sdr.Read())
                //        {
                //            UserDetail UD = new UserDetail();
                //            string FullName = "";
                //            string Fname= sdr["fname"].ToString();
                //            string Lname = sdr["lname"].ToString();
                //             FullName = Fname+" " + Lname;
                //            UD.Name = FullName;
                //            UD.Id = Convert.ToInt32(sdr["Id"]);
                //            UD.City = sdr["City"].ToString();
                //            UD.DOB = Convert.ToDateTime(sdr["dob"]);
                //            UD.Address = sdr["address"].ToString();
                //            UD.RegDate = Convert.ToDateTime(sdr["regDateTime"]);

                //            li.Add(UD);


                //        }
                //    }
                //    return li;


                //}

                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", "ProductAndTestCompleteCand");
                    List<UserDetail> li = new List<UserDetail>();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            UserDetail UD = new UserDetail();
                            string FullName = "";
                            string Fname = sdr["fname"].ToString();
                            string Lname = sdr["lname"].ToString();
                            FullName = Fname + " " + Lname;
                            UD.Name = FullName;
                            UD.Id = Convert.ToInt32(sdr["Id"]);
                            UD.City = sdr["City"].ToString();
                            //  UD.DOB = Convert.ToDateTime(sdr["dob"]);

                            string Birth = sdr["dob"].ToString();
                            string[] birthdate = Birth.Split(' ');
                            UD.BirthDate = birthdate[0].ToString();

                            UD.Address = sdr["address"].ToString();
                            UD.RegDate = Convert.ToDateTime(sdr["regDateTime"]);
                            UD.Gender = sdr["gender"].ToString();
                            UD.Contact = sdr["contactNo"].ToString();
                            UD.EmailId = sdr["email"].ToString();
                            UD.Pincode = sdr["pincode"].ToString();
                            UD.UserStatus = sdr["userStatus"].ToString();
                            UD.TestStatus = sdr["teststatus"].ToString();
                            li.Add(UD);
                        }
                    }
                    return li;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool PostRegistration(UserDetail UD)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    SqlCommand cmd = new SqlCommand("spUserRegistration", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserTypeId", UD.UserTypeId);
                    cmd.Parameters.AddWithValue("@Fname", UD.Fname);
                    cmd.Parameters.AddWithValue("@Lname", UD.Lname);
                    cmd.Parameters.AddWithValue("@Gender", UD.Gender);
                    cmd.Parameters.AddWithValue("@DBO", UD.DOB);
                    cmd.Parameters.AddWithValue("@ContactNo", UD.Contact);
                    cmd.Parameters.AddWithValue("@Email", UD.EmailId);
                    cmd.Parameters.AddWithValue("@CityId", UD.CityId);
                    cmd.Parameters.AddWithValue("@Pincode", UD.Pincode);
                    cmd.Parameters.AddWithValue("@Address", UD.Address);
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        return true;
                    }
                    else
                    {

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<StudentDetails> GetStudentDetails()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    
                    SqlCommand cmd = new SqlCommand("spGetStudentDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<StudentDetails> li = new List<StudentDetails>();
                    con.Open();                    
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if(sdr.HasRows)
                    {
                        while(sdr.Read())
                        {
                            StudentDetails stud = new StudentDetails();
                            int UserType = Convert.ToInt32(sdr["UserTypeId"]);
                            if (UserType == 5)
                            {
                                stud.LeadType = "Student";
                            }
                            else {
                                stud.LeadType = "Professional";
                            }

                            stud.Id = Convert.ToInt32(sdr["Id"]);
                            stud.fname= sdr["fname"].ToString();
                            stud.lname = sdr["lname"].ToString();
                            stud.Gender = sdr["Gender"].ToString();
                            stud.Email = sdr["email"].ToString();
                            stud.contactNo = sdr["contactNo"].ToString();

                            stud.birth = sdr["dob"].ToString();
                            if (stud.birth != null && stud.birth.ToString().Length > 0)
                            {
                                stud.dob = Convert.ToDateTime(sdr["dob"]);
                            }
                            else
                            {

                            }

                            stud.dob = Convert.ToDateTime(sdr["dob"]);
                            //stud.dob = Convert.ToDateTime(sdr["dob"]);
                            stud.regDateTime = Convert.ToDateTime(sdr["regDateTime"]);
                            stud.pincode = sdr["pincode"].ToString();
                            stud.address = sdr["address"].ToString();
                            stud.city = sdr["city"].ToString();                            
                            stud.userStatus = sdr["userstatus"].ToString();
                            stud.productStatus = sdr["productstatus"].ToString();
                            stud.TestStatus = sdr["teststatus"].ToString();
                            stud.SessionStatus = sdr["sessionstatus"].ToString();
                            stud.CDFAssignedStatus = sdr["CDFassignedstatus"].ToString();
                            stud.ShadowCDFAssignedStatus = sdr["shadowCDFassignedstatus"].ToString();                          


                            DateTime today = System.DateTime.Now.Date;
                            DateTime datetime = stud.regDateTime.Date;                          
                            DateTime yesterday = DateTime.Today.AddDays(-1);
                           if (datetime.CompareTo(today)==0)
                            {
                                stud.finaldate = "Today";
                            }
                           else if(yesterday.CompareTo(datetime)==0)
                            {
                                stud.finaldate = "Yesterday";
                            }
                            else
                            {
                                stud.regDateTime = Convert.ToDateTime(sdr["regDateTime"]);
                            }

                            string AcceptStatus = sdr["CDFAcceptanceStatus"].ToString();

                            if (AcceptStatus == "")
                            {
                                stud.CDFAcceptanceStatus = "NotAvail";
                            }
                            //if (AcceptStatus == "False")
                            //{
                            //    stud.CDFAcceptanceStatus = "zero";
                            //}
                            //if (AcceptStatus == "True")
                            //{
                            //    stud.CDFAcceptanceStatus = "one";
                            //}
                            else
                            {
                                stud.CDFAcceptanceStatus = AcceptStatus;
                            }

                            //stud.CDFAcceptanceStatus =Convert.ToBoolean(sdr["CDFAcceptanceStatus"]);

                            //   string aa = sdr["CDFAcceptanceStatus"].ToString();

                            li.Add(stud);

                            List<string> list = new List<string>();
                            if(stud.productStatus== "ProdPurchase")
                            {
                                list.Add("ProdPurchase");                                
                            }
                           else if (stud.TestStatus == "Complete") 
                            {
                                list.Add("Complete");
                            }
                            else if (stud.SessionStatus== "Assigned")
                            {
                                list.Add("Assigned");
                            }
                           
                            
                        }
                    }
                    return li;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}