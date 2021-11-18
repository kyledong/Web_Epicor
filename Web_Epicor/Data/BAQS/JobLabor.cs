using System;
using System.Data;
using System.Data.SqlClient;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using Web_Epicor.Data.Procedures;
using Web_Epicor.Data.AppJson;
using Web_Epicor.Results;

namespace Web_Epicor.Data.BAQS
{
    public class JobLabor
    {
        public static void getLaborTime()
        {
            try
            {
                var client = new RestClient(LoadJsonData.LaborTime_url());
                client.Authenticator = new NtlmAuthenticator(LoadCredentials.Email(), LoadCredentials.Password());
                var request = new RestRequest(Method.GET);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Accept", "application/json");

                IRestResponse response = client.Execute(request);
                var content = response.Content;

                int index = content.IndexOf(Environment.NewLine);
                string newText = content.Substring(index + Environment.NewLine.Length);
                int index2 = newText.IndexOf(Environment.NewLine);
                string newText2 = newText.Substring(index2 + Environment.NewLine.Length);
                newText2 = newText2.Remove(newText2.LastIndexOf(Environment.NewLine));
                newText2 = "[" + newText2.Replace(Environment.NewLine, "");

                DataTable dsTopics = JsonConvert.DeserializeObject<DataTable>(newText2);

                SqlConnection cn = new SqlConnection(LoadJsonData.ConnetionString());
                SqlBulkCopy objBulk = new SqlBulkCopy(cn);
                objBulk.DestinationTableName = "JOBS_Labor";

                objBulk.ColumnMappings.Add("LaborDtl_LaborDtlSeq", "LaborDtl_LaborDtlSeq");
                objBulk.ColumnMappings.Add("EmpBasic_Name", "EmpBasic_Name");
                objBulk.ColumnMappings.Add("LaborDtl_EmployeeNum", "LaborDtl_EmployeeNum");
                objBulk.ColumnMappings.Add("LaborDtl_OpCode", "LaborDtl_OpCode");
                objBulk.ColumnMappings.Add("LaborDtl_JobNum", "LaborDtl_JobNum");
                objBulk.ColumnMappings.Add("LaborDtl_ClockInDate", "LaborDtl_ClockInDate");
                objBulk.ColumnMappings.Add("LaborDtl_DspClockInTime", "LaborDtl_DspClockInTime");
                objBulk.ColumnMappings.Add("LaborDtl_DspClockOutTime", "LaborDtl_DspClockOutTime");
                objBulk.ColumnMappings.Add("LaborDtl_LaborRate", "LaborDtl_LaborRate");
                objBulk.ColumnMappings.Add("LaborDtl_BurdenRate", "LaborDtl_BurdenRate");
                objBulk.ColumnMappings.Add("LaborDtl_LaborHrs", "LaborDtl_LaborHrs");
                objBulk.ColumnMappings.Add("LaborDtl_BurdenHrs", "LaborDtl_BurdenHrs");
                objBulk.ColumnMappings.Add("Calculated_LaborCost", "Calculated_LaborCost");
                objBulk.ColumnMappings.Add("Calculated_BurdenCost", "Calculated_BurdenCost");
                objBulk.ColumnMappings.Add("LaborDtl_ResourceGrpID", "LaborDtl_ResourceGrpID");
                objBulk.ColumnMappings.Add("LaborDtl_ResourceID", "LaborDtl_ResourceID");
                objBulk.ColumnMappings.Add("LaborDtl_CreateDate", "LaborDtl_CreateDate");
                objBulk.ColumnMappings.Add("LaborDtl_ApprovedDate", "LaborDtl_ApprovedDate");
                objBulk.ColumnMappings.Add("LaborDtl_ChangeDate", "LaborDtl_ChangeDate");
                objBulk.ColumnMappings.Add("LaborDtl_ChangedBy", "LaborDtl_ChangedBy");
                objBulk.ColumnMappings.Add("LaborDtl_CreateTime", "LaborDtl_CreateTime");
                objBulk.ColumnMappings.Add("LaborDtl_LaborQty", "LaborDtl_LaborQty");
                objBulk.ColumnMappings.Add("LaborDtl_OprSeq", "LaborDtl_OprSeq");

                cn.Open();
                objBulk.WriteToServer(dsTopics);
                cn.Close();

                UpdateDate.updateJobs(2);
                SuccessfulLog.SaveFile("Job labor");

            }
            catch (Exception ex)
            {
                ErrorLog.SaveFile("Job_Labor", ex);
                ErrorLog.SendMail("Job Labor", ex);
            }
        }


        public static void GetJobLaborV1()
        {
            try
            {
                var client = new RestClient(LoadJsonData.LaborTime_url());
                client.Authenticator = new NtlmAuthenticator(LoadCredentials.Email(), LoadCredentials.Password());
                var request = new RestRequest(Method.GET);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Accept", "application/json");

                IRestResponse response = client.Execute(request);
                var content = response.Content;

                int index = content.IndexOf(Environment.NewLine);
                string newText = content.Substring(index + Environment.NewLine.Length);
                int index2 = newText.IndexOf(Environment.NewLine);
                string newText2 = newText.Substring(index2 + Environment.NewLine.Length);
                newText2 = newText2.Remove(newText2.LastIndexOf(Environment.NewLine));
                newText2 = "[" + newText2.Replace(Environment.NewLine, "");

                DataTable dsTopics = JsonConvert.DeserializeObject<DataTable>(newText2);

                foreach (DataRow r in dsTopics.Rows)
                {
                    try
                    {
                        string sqltxt = "INSERT INTO[Labor_Time] (EmpBasicName, LaborDtl_EmployeeNum, LaborDtl_OpCode, LaborDtl_JobNum, LaborDtl_ClockInDate, LaborDtl_DspClockInTime," +
                          " LaborDtl_DspClockOutTime, LaborDtl_LaborRate, LaborDtl_BurdenRate, LaborDtl_LaborHrs, LaborDtl_BurdenHrs, Calculated_LaborCost, Calculated_BurdenCost, " +
                          "LaborDtl_ResourceGrpID, LaborDtl_ResourceID, LaborDtl_CreateDate, LaborDtl_ApprovedDate, LaborDtl_ChangeDate, LaborDtl_ChangedBy, LaborDtl_CreateTime, " +
                          " LaborDtl_LaborQty, LaborDtl_OprSeq, LAST_RUN)" +
                          " VALUES('" + r["EmpBasic_Name"] + "', '" + r["LaborDtl_EmployeeNum"] + "', '" + r["LaborDtl_OpCode"] + "', '" + r["LaborDtl_JobNum"] + "', @LaborDtl_ClockInDate, '" +
                          r["LaborDtl_DspClockInTime"] + "', '" + r["LaborDtl_DspClockOutTime"] + "', '" + r["LaborDtl_LaborRate"] + "', '" + r["LaborDtl_BurdenRate"] + "', '" + r["LaborDtl_LaborHrs"] + "', '"
                          + r["LaborDtl_BurdenHrs"] + "', '" + r["Calculated_LaborCost"] + "', '" + r["Calculated_BurdenCost"] + "', '" + r["LaborDtl_ResourceGrpID"] + "', '" + r["LaborDtl_ResourceID"] + "',"
                          + "@LaborDtl_CreateDate, @LaborDtl_ApprovedDate,  @LaborDtl_ChangeDate" + ", '" + r["LaborDtl_ChangedBy"] + "', '" + r["LaborDtl_CreateTime"] + "', '"
                          + r["LaborDtl_LaborQty"] + "', '" + r["LaborDtl_CreateTime"] + "', @LAST_RUN)";

                        var sqlcon = new SqlConnection(LoadJsonData.ConnetionString()); // Json conexión 
                        var comando = new SqlCommand(sqltxt, sqlcon);

                        try
                        {
                            comando.Connection.Open();
                            comando.Parameters.Clear();
                            comando.Parameters.AddWithValue("@LaborDtl_ClockInDate", r["LaborDtl_ClockInDate"]);
                            comando.Parameters.AddWithValue("@LaborDtl_CreateDate", r["LaborDtl_CreateDate"]);
                            comando.Parameters.AddWithValue("@LaborDtl_ApprovedDate", r["LaborDtl_ApprovedDate"]);
                            comando.Parameters.AddWithValue("@LaborDtl_ChangeDate", r["LaborDtl_ChangeDate"]);
                            comando.Parameters.AddWithValue("@Last_RUN", DateTime.Now);
                            comando.ExecuteNonQuery();
                            comando.Connection.Close();
                        }
                        catch (Exception ex)
                        {
                            comando.Connection.Close();

                            string sqltxt2 = "INSERT INTO [dbo].[error]( part_num, record, name_table, error, run_at, query) VALUES (@part_num, @record, @name_table, @error, @run_at, @query)";
                            var comando2 = new SqlCommand(sqltxt2, sqlcon);

                            try
                            {

                                string records = r["EmpBasic_Name"] + "', '" + r["LaborDtl_EmployeeNum"] + "', '" + r["LaborDtl_OpCode"] + "', '" + r["LaborDtl_JobNum"] + "', '" + r["LaborDtl_ClockInDate"] + "', '"
                                 + r["LaborDtl_DspClockInTime"] + "', '" + r["LaborDtl_DspClockOutTime"] + "', '" + r["LaborDtl_LaborRate"] + "', '" + r["LaborDtl_BurdenRate"] + "', '" + r["LaborDtl_LaborHrs"] + "', '"
                                 + r["LaborDtl_BurdenHrs"] + "', '" + r["Calculated_LaborCost"] + "', '" + r["Calculated_BurdenCost"] + "', '" + r["LaborDtl_ResourceGrpID"] + "', '" + r["LaborDtl_ResourceID"] + "', '"
                                 + r["LaborDtl_CreateDate"] + "', '" + r["LaborDtl_ApprovedDate"] + "', '" + r["LaborDtl_ChangeDate"] + "', '" + r["LaborDtl_ChangedBy"] + "', '" + r["LaborDtl_CreateTime"] + "', '"
                                 + r["LaborDtl_LaborQty"];

                                comando2.Connection.Open();
                                comando2.Parameters.Clear();
                                comando2.Parameters.AddWithValue("@part_num", "N/A");
                                comando2.Parameters.AddWithValue("@record", records);
                                comando2.Parameters.AddWithValue("@name_table", "JOBMatrials");
                                comando2.Parameters.AddWithValue("@error", ex.Message);
                                comando2.Parameters.AddWithValue("@run_at", DateTime.Now);
                                comando2.Parameters.AddWithValue("@query", sqltxt);
                                comando2.ExecuteNonQuery();
                                comando2.Connection.Close();
                            }

                            catch (Exception ex2)
                            {

                                comando2.Connection.Close();
                                ErrorLog.SaveFile("Job_Labor", ex2);//
                                ErrorLog.SendMail("Job Labor", ex2);

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        ErrorLog.SaveFile("Job_Labor", ex);
                        ErrorLog.SendMail("Job Labor", ex);
                    }

                }
                UpdateDate.updateJobs(2);
                SuccessfulLog.SaveFile("Job labor");

            }
            catch (Exception ex)
            {
                ErrorLog.SaveFile("Job_Labor", ex);
                ErrorLog.SendMail("Job Labor", ex);
            }
        }
    }
}
