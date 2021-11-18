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
    public class JobMaterials
    {
        public static void GetJobMAterials()
        {
            try
            {
                var client = new RestClient(LoadJsonData.JobMaterials_url());

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
                objBulk.DestinationTableName = "JOBS_MATERIALS";

                objBulk.ColumnMappings.Add("JobHead_CreateDate", "JobHead_CreateDate");
                objBulk.ColumnMappings.Add("JobHead_JobNum", "JobHead_JobNum");
                objBulk.ColumnMappings.Add("JobHead_JobComplete", "JobHead_JobComplete");
                objBulk.ColumnMappings.Add("JobHead_JobClosed", "JobHead_JobClosed");
                objBulk.ColumnMappings.Add("JobOper_OpComplete", "JobOper_OpComplete");
                objBulk.ColumnMappings.Add("JobMtl_MtlSeq", "JobMtl_MtlSeq");
                objBulk.ColumnMappings.Add("JobMtl_AssemblySeq", "JobMtl_AssemblySeq");
                objBulk.ColumnMappings.Add("JobMtl_PartNum", "JobMtl_PartNum");
                objBulk.ColumnMappings.Add("JobMtl_Description", "JobMtl_Description");
                objBulk.ColumnMappings.Add("JobMtl_IUM", "JobMtl_IUM");
                objBulk.ColumnMappings.Add("JobMtl_RequiredQty", "JobMtl_RequiredQty");
                objBulk.ColumnMappings.Add("JobMtl_IssuedQty", "JobMtl_IssuedQty");
                objBulk.ColumnMappings.Add("JobMtl_TotalCost", "JobMtl_TotalCost");
                objBulk.ColumnMappings.Add("JobMtl_IssuedComplete", "JobMtl_IssuedComplete");
                objBulk.ColumnMappings.Add("JobOper_OprSeq", "JobOper_OprSeq");
                objBulk.ColumnMappings.Add("JobOper_OpDesc", "JobOper_OpDesc");
                objBulk.ColumnMappings.Add("JobMtl_BackFlush", "JobMtl_BackFlush");
                objBulk.ColumnMappings.Add("JobMtl_BuyIt", "JobMtl_BuyIt");
                objBulk.ColumnMappings.Add("JobMtl_Ordered", "JobMtl_Ordered");
                objBulk.ColumnMappings.Add("OrderRel_ReqDate","OrderRel_ReqDate");

                cn.Open();
                objBulk.WriteToServer(dsTopics);
                cn.Close();
            
                UpdateDate.updateJobs(3);
                SuccessfulLog.SaveFile("Job materials");
            }
            catch (Exception ex)
            {
                ErrorLog.SaveFile("Job Materials", ex);
                ErrorLog.SendMail("Job Materials", ex);
            }
        }
        public static void GetJobMAterialsV1()
        {
            try
            {

                var client = new RestClient(LoadJsonData.JobMaterials_url());

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

                        string sqltxt = "  INSERT INTO JOBS_MATERIALS " +
                         " (JobHead_JobNum, JobHead_JobComplete, JobHead_JobClosed, JobMtl_MtlSeq, JobMtl_AssemblySeq, JobMtl_PartNum, JobMtl_Description, JobMtl_IUM, JobMtl_RequiredQty, JobMtl_IssuedQty, " +
                         " JobMtl_TotalCost, JobMtl_IssuedComplete, JobOper_OprSeq, JobOper_OpDesc, JobMtl_BackFlush, JobMtl_BuyIt, JobMtl_Ordered, OrderRel_ReqDate, Last_run) " +
                         " values(" + r["JobHead_JobNum"] + ",'" + r["JobHead_JobComplete"] + "','" + r["JobHead_JobClosed"] + "','" + r["JobMtl_MtlSeq"] + "','" + r["JobMtl_AssemblySeq"] + "','" + r["JobMtl_PartNum"] + "','" + r["JobMtl_Description"] +
                         "','" + r["JobMtl_IUM"] + "'," + r["JobMtl_RequiredQty"] + "," + r["JobMtl_IssuedQty"] + "," + r["JobMtl_TotalCost"] + ",'" + r["JobMtl_IssuedComplete"] + "'," + r["JobOper_OprSeq"] + ",'" + r["JobOper_OpDesc"] +
                         "','" + r["JobMtl_BackFlush"] + "','" + r["JobMtl_BuyIt"] + "','" + r["JobMtl_Ordered"] + "',@OrderRel_ReqDate, @Last_RUN)";


                        var sqlcon = new SqlConnection(LoadJsonData.ConnetionString());
                        var comando = new SqlCommand(sqltxt, sqlcon);

                        try
                        {
                            comando.Connection.Open();

                            comando.Parameters.Clear();

                            comando.Parameters.AddWithValue("@OrderRel_ReqDate", r["OrderRel_ReqDate"]);
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
                                string records = r["JobHead_JobNum"] + "','" + r["JobHead_JobComplete"] + "','" + r["JobHead_JobClosed"] + "','" + r["JobMtl_MtlSeq"] + "','" + r["JobMtl_AssemblySeq"] + "','" + r["JobMtl_PartNum"] + "','" + r["JobMtl_Description"] +
                              "','" + r["JobMtl_IUM"] + "','" + r["JobMtl_RequiredQty"] + "','" + r["JobMtl_IssuedQty"] + "','" + r["JobMtl_TotalCost"] + "','" + r["JobMtl_IssuedComplete"] + "','" + r["JobOper_OprSeq"] + "','" + r["JobOper_OpDesc"] +
                               "','" + r[" JobMtl_BackFlush"] + "','" + r["JobMtl_BuyIt"] + "','" + r["JobMtl_Ordered"] + r["OrderRel_ReqDate"];


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
                                ErrorLog.SaveFile("Job Materials", ex2);
                                ErrorLog.SendMail("Job Materials", ex2);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.SaveFile("Job Materials", ex);
                        ErrorLog.SendMail("Job Materials", ex);
                    }

                }
                UpdateDate.updateJobs(3);
                SuccessfulLog.SaveFile("Job materials");
            }
            catch (Exception ex)
            {
                ErrorLog.SaveFile("Job Materials", ex);
                ErrorLog.SendMail("Job Materials", ex);
            }
        }
    }
}
