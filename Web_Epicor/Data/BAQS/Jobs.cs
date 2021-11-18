using System;
using System.Data;
using System.Data.SqlClient;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using Web_Epicor.Data.Procedures;
using Web_Epicor.Data.AppJson;
using Web_Epicor.Results;
using System.Globalization;

namespace Web_Epicor.Data.BAQS
{
    public class Jobs
    {
        public static void GetJobs()
        {

            
            try
            {
                var client = new RestClient(LoadJsonData.Job_url());
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

                        string sqltxt = "INSERT INTO JOBS(JobHead_CreateDate, OrderHed_OrderNum, OrderHed_OrderDate, Customer_CustID, Customer_Name, OrderDtl_PartNum, OrderDtl_OrderLine, OrderRel_OrderRelNum, JobHead_DueDate, " +
                                 " JobProd_JobNum, JobProd_ProdQty, JobHead_JobClosed, JobHead_JobComplete, JobHead_JobCompletionDate, JobHead_LastChangedOn, JobHead_LastChangedBy, Last_RUN) " +
                                "  VALUES(@JobHead_CreateDate," + r["OrderHed_OrderNum"] + ", @OrderHed_OrderDate,'" + r["Customer_CustID"] + "','" + r["Customer_Name"] + "','" + r["OrderDtl_PartNum"] + "'," + r["OrderDtl_OrderLine"] + "," +
                                   r["OrderRel_OrderRelNum"] + ",@JobHead_DueDate,'" + r["JobProd_JobNum"] + "'," + r["JobProd_ProdQty"] + ",'" + r["JobHead_JobClosed"] + "','" +
                                         r["JobHead_JobComplete"] + "',@JobHead_JobCompletionDate, @JobHead_LastChangedOn, @JobHead_LastChangedBy, @Last_RUN )";


                        var sqlcon = new SqlConnection(LoadJsonData.ConnetionString());
                        var comando = new SqlCommand(sqltxt, sqlcon);

                        try
                        {
                            comando.Connection.Open();
                            comando.Parameters.Clear();
                            comando.Parameters.AddWithValue("@JobHead_CreateDate", r["JobHead_CreateDate"]);
                            comando.Parameters.AddWithValue("@OrderHed_OrderDate", r["OrderHed_OrderDate"]);                            
                            comando.Parameters.AddWithValue("@JobHead_DueDate", r["JobHead_DueDate"]);
                            comando.Parameters.AddWithValue("@JobHead_JobCompletionDate", r["JobHead_JobCompletionDate"]);
                            comando.Parameters.AddWithValue("@JobHead_LastChangedOn", r["JobHead_LastChangedOn"]);
                            comando.Parameters.AddWithValue("@JobHead_LastChangedBy", r["JobHead_LastChangedBy"]);
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
                                string records = r["OrderHed_OrderNum"] + "," + r["OrderHed_OrderDate"] + "," + r["Customer_CustID"] + "," + r["Customer_Name"] + "," + r["OrderDtl_PartNum"] + "," + r["OrderDtl_OrderLine"] + "," +
                                   r["OrderRel_OrderRelNum"] + "," + r["JobHead_CreateDate"] + "," + r["JobHead_DueDate"] + "," + r["JobProd_JobNum"] + "," + r["JobProd_ProdQty"] + "," + r["JobHead_JobClosed"] + "," +
                                         r["JobHead_JobComplete"] + "," + r["JobHead_JobCompletionDate"] + ", " + r["JobHead_LastChangedOn"] + ", " + r["JobHead_LastChangedBy"];



                                comando2.Connection.Open();
                                comando2.Parameters.Clear();

                                comando2.Parameters.AddWithValue("@part_num", "N/A");
                                comando2.Parameters.AddWithValue("@record", records);
                                comando2.Parameters.AddWithValue("@name_table", "JOBS");
                                comando2.Parameters.AddWithValue("@error", ex.Message);
                                comando2.Parameters.AddWithValue("@run_at", DateTime.Now);
                                comando2.Parameters.AddWithValue("@query", sqltxt);
                                comando2.ExecuteNonQuery();
                                comando2.Connection.Close();


                            }

                            catch (Exception ex2)
                            {
                                comando2.Connection.Close();
                                ErrorLog.SaveFile("Job", ex2);
                                ErrorLog.SendMail("Job", ex2);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.SaveFile("Job", ex);
                        ErrorLog.SendMail("Job", ex);

                    }
                }

                UpdateDate.updateJobs(1);
                SuccessfulLog.SaveFile("Jobs");
            }
            catch(Exception ex)
            {
                ErrorLog.SaveFile("Job", ex);
                ErrorLog.SendMail("Job", ex);
            }
        }

        public static void GetJobsParameters(string param)
        {


            try
            {
                var client = new RestClient(LoadJsonData.Job_url() +"?$filter="+ param);
                string ejem = LoadJsonData.Job_url() + "?$filter=" + param;

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

                        string sqltxt = "INSERT INTO JOBS(JobHead_CreateDate, OrderHed_OrderNum, OrderHed_OrderDate, Customer_CustID, Customer_Name, OrderDtl_PartNum, OrderDtl_OrderLine, OrderRel_OrderRelNum, JobHead_DueDate, " +
                                   " JobProd_JobNum, JobProd_ProdQty, JobHead_JobClosed, JobHead_JobComplete, JobHead_JobCompletionDate, JobHead_LastChangedOn, JobHead_LastChangedBy, Last_RUN) " +
                                  "  VALUES(@JobHead_CreateDate," + r["OrderHed_OrderNum"] + ", @OrderHed_OrderDate,'" + r["Customer_CustID"] + "','" + r["Customer_Name"] + "','" + r["OrderDtl_PartNum"] + "'," + r["OrderDtl_OrderLine"] + "," +
                                     r["OrderRel_OrderRelNum"] + ",@JobHead_DueDate,'" + r["JobProd_JobNum"] + "'," + r["JobProd_ProdQty"] + ",'" + r["JobHead_JobClosed"] + "','" +
                                           r["JobHead_JobComplete"] + "',@JobHead_JobCompletionDate, @JobHead_LastChangedOn, @JobHead_LastChangedBy, @Last_RUN )";


                        var sqlcon = new SqlConnection(LoadJsonData.ConnetionString());
                        var comando = new SqlCommand(sqltxt, sqlcon);

                        try
                        {
                            comando.Connection.Open();
                            comando.Parameters.Clear();
                            comando.Parameters.AddWithValue("@JobHead_CreateDate", r["JobHead_CreateDate"]);
                            comando.Parameters.AddWithValue("@OrderHed_OrderDate", r["OrderHed_OrderDate"]);                           
                            comando.Parameters.AddWithValue("@JobHead_DueDate", r["JobHead_DueDate"]);
                            comando.Parameters.AddWithValue("@JobHead_JobCompletionDate", r["JobHead_JobCompletionDate"]);
                            comando.Parameters.AddWithValue("@JobHead_LastChangedOn", r["JobHead_LastChangedOn"]);
                            comando.Parameters.AddWithValue("@JobHead_LastChangedBy", r["JobHead_LastChangedBy"]);
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
                                string records = r["OrderHed_OrderNum"] + "," + r["OrderHed_OrderDate"] + "," + r["Customer_CustID"] + "," + r["Customer_Name"] + "," + r["OrderDtl_PartNum"] + "," + r["OrderDtl_OrderLine"] + "," +
                                   r["OrderRel_OrderRelNum"] + "," + r["JobHead_CreateDate"] + "," + r["JobHead_DueDate"] + "," + r["JobProd_JobNum"] + "," + r["JobProd_ProdQty"] + "," + r["JobHead_JobClosed"] + "," +
                                         r["JobHead_JobComplete"] + "," + r["JobHead_JobCompletionDate"] + ", " + r["JobHead_LastChangedOn"] + ", " + r["JobHead_LastChangedBy"];



                                comando2.Connection.Open();
                                comando2.Parameters.Clear();

                                comando2.Parameters.AddWithValue("@part_num", "N/A");
                                comando2.Parameters.AddWithValue("@record", records);
                                comando2.Parameters.AddWithValue("@name_table", "JOBS");
                                comando2.Parameters.AddWithValue("@error", ex.Message);
                                comando2.Parameters.AddWithValue("@run_at", DateTime.Now);
                                comando2.Parameters.AddWithValue("@query", sqltxt);
                                comando2.ExecuteNonQuery();
                                comando2.Connection.Close();


                            }

                            catch (Exception ex2)
                            {
                                comando2.Connection.Close();
                                ErrorLog.SaveFile("Job", ex2);
                                ErrorLog.SendMail("Job", ex2);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.SaveFile("Job", ex);
                        ErrorLog.SendMail("Job", ex);

                    }
                }

                UpdateDate.updateJobs(1);
                SuccessfulLog.SaveFile("Jobs");
            }
            catch (Exception ex)
            {
                ErrorLog.SaveFile("Job", ex);
                ErrorLog.SendMail("Job", ex);
            }
        }


        public static void GetJobsBulk()
        {


            try
            {
                var client = new RestClient(LoadJsonData.Job_url());
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

                
                SqlConnection con = new SqlConnection(LoadJsonData.ConnetionString());
                SqlBulkCopy objBulk = new SqlBulkCopy(con);
                objBulk.DestinationTableName = "JOBS";


                
                objBulk.ColumnMappings.Add("JobHead_CreateDate", "JobHead_CreateDate");
                objBulk.ColumnMappings.Add("OrderHed_OrderNum", "OrderHed_OrderNum");
                objBulk.ColumnMappings.Add("OrderHed_OrderDate", "OrderHed_OrderDate");
                objBulk.ColumnMappings.Add("Customer_CustID", "Customer_CustID");
                objBulk.ColumnMappings.Add("Customer_Name", "Customer_Name");
                objBulk.ColumnMappings.Add("OrderDtl_PartNum", "OrderDtl_PartNum");
                objBulk.ColumnMappings.Add("OrderDtl_OrderLine", "OrderDtl_OrderLine");
                objBulk.ColumnMappings.Add("OrderRel_OrderRelNum", "OrderRel_OrderRelNum");
                objBulk.ColumnMappings.Add("JobHead_DueDate", "JobHead_DueDate");
                objBulk.ColumnMappings.Add("JobProd_JobNum", "JobProd_JobNum");
                objBulk.ColumnMappings.Add("JobProd_ProdQty", "JobProd_ProdQty");
                objBulk.ColumnMappings.Add("JobHead_JobClosed", "JobHead_JobClosed");
                objBulk.ColumnMappings.Add("JobHead_JobComplete", "JobHead_JobComplete");
                objBulk.ColumnMappings.Add("JobHead_JobCompletionDate", "JobHead_JobCompletionDate");
                objBulk.ColumnMappings.Add("JobHead_LastChangedOn", "JobHead_LastChangedOn");
                objBulk.ColumnMappings.Add("JobHead_LastChangedBy", "JobHead_LastChangedBy");
                

                con.Open();
                objBulk.WriteToServer(dsTopics);
                con.Close();


                UpdateDate.updateJobs(1);
                SuccessfulLog.SaveFile("Jobs");
            }
            catch (Exception ex)
            {
                ErrorLog.SaveFile("Job", ex);
                ErrorLog.SendMail("Job", ex);
            }
        }
    }
}
