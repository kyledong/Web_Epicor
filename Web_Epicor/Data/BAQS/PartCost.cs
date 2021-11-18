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
    public class PartCost
    {
        public static void GetPart_cost()
        {

            try
            {
                var client = new RestClient(LoadJsonData.PartCost_url());
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
                objBulk.DestinationTableName = "PART_COST";

                objBulk.ColumnMappings.Add("Part_ChangedOn", "Part_ChangedOn");
                objBulk.ColumnMappings.Add("Part_PartNum", "Part_PartNum");
                objBulk.ColumnMappings.Add("Part_PartDescription", "Part_PartDescription");
                objBulk.ColumnMappings.Add("ProdGrup_Description", "ProdGrup_Description");
                objBulk.ColumnMappings.Add("Part_ClassID", "Part_ClassID");
                objBulk.ColumnMappings.Add("PartCost_AvgLaborCost", "PartCost_AvgLaborCost");
                objBulk.ColumnMappings.Add("PartCost_AvgBurdenCost", "PartCost_AvgBurdenCost");
                objBulk.ColumnMappings.Add("PartCost_AvgMaterialCost", "PartCost_AvgMaterialCost");
                objBulk.ColumnMappings.Add("PartCost_AvgSubContCost", "PartCost_AvgSubContCost");
                objBulk.ColumnMappings.Add("PartCost_AvgMtlBurCost","PartCost_AvgMtlBurCost");
                objBulk.ColumnMappings.Add("Calculated_AVGUnitCost", "Calculated_AVGUnitCost");
                objBulk.ColumnMappings.Add("PartCost_StdLaborCost", "PartCost_StdLaborCost");
                objBulk.ColumnMappings.Add("PartCost_StdBurdenCost", "PartCost_StdBurdenCost");
                objBulk.ColumnMappings.Add("PartCost_StdMaterialCost", "PartCost_StdMaterialCost");
                objBulk.ColumnMappings.Add("PartCost_StdSubContCost", "PartCost_StdSubContCost");
                objBulk.ColumnMappings.Add("PartCost_StdMtlBurCost", "PartCost_StdMtlBurCost");
                objBulk.ColumnMappings.Add("Part_NonStock", "Part_NonStock");
                objBulk.ColumnMappings.Add("Part_TypeCode", "Part_TypeCode");
                objBulk.ColumnMappings.Add("Calculated_StdUnitCost", "Calculated_StdUnitCost");
                objBulk.ColumnMappings.Add("Part_CreatedOn", "Part_CreatedOn");

                cn.Open();
                objBulk.WriteToServer(dsTopics);
                cn.Close();
            
                UpdateDate.updateJobs(4);
                SuccessfulLog.SaveFile("Part cost");
                //

            }
            catch(Exception ex)
            {
                ErrorLog.SaveFile("Part Cost", ex);
                ErrorLog.SendMail("Part Cost", ex);
            }

        }


        public static void GetPart_costV1()
        {

            try
            {


                var client = new RestClient(LoadJsonData.PartCost_url());
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
                        string sqltxt = " INSERT INTO   PART_COST(Part_PartNum, Part_PartDescription, ProdGrup_Description, Part_ClassID, PartCost_AvgLaborCost, PartCost_AvgBurdenCost, PartCost_AvgMaterialCost, PartCost_AvgSubContCost, " +
                                " PartCost_AvgMtlBurCost, Calculated_AVGUnitCost, PartCost_StdLaborCost, PartCost_StdBurdenCost, PartCost_StdMaterialCost, PartCost_StdSubContCost, PartCost_StdMtlBurCost, Part_NonStock, " +
                                " Part_TypeCode, Calculated_StdUnitCost, Part_CreatedOn, Part_ChangedOn, run_time) " +
                                " VALUES ('" + r["Part_PartNum"] + "','" + r["ProdGrup_Description"] + "','" + r["ProdGrup_Description"] + "', '" + r["Part_ClassID"] + "'," + r["PartCost_AvgLaborCost"] + "," + r["PartCost_AvgBurdenCost"] +
                                "," + r["PartCost_AvgMaterialCost"] + "," + r["PartCost_AvgSubContCost"] + "," + r["PartCost_AvgMtlBurCost"] + "," + r["Calculated_AVGUnitCost"] + "," + r["PartCost_StdLaborCost"] + "," + r["PartCost_StdBurdenCost"] +
                                "," + r["PartCost_StdMaterialCost"] + "," + r["PartCost_StdSubContCost"] + "," + r["PartCost_StdMtlBurCost"] + ",'" + r["Part_NonStock"] + "','" + r["Part_TypeCode"] + "'," + r["Calculated_StdUnitCost"] + ",@Part_CreatedOn, @Part_ChangedOn, @run_time )";



                        var sqlcon = new SqlConnection(LoadJsonData.ConnetionString());
                        var comando = new SqlCommand(sqltxt, sqlcon);


                        try
                        {
                            comando.Connection.Open();
                            comando.Parameters.Clear();
                            comando.Parameters.AddWithValue("@Part_CreatedOn", r["Part_CreatedOn"]);
                            comando.Parameters.AddWithValue("@Part_ChangedOn", r["Part_ChangedOn"]);
                            comando.Parameters.AddWithValue("@run_time", DateTime.Now);
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
                                string records = r["Part_PartNum"] + "," + r["Part_PartDescription"] + "," + r["ProdGrup_Description"] + "," + r["Part_ClassID"] + "," + r["PartCost_AvgLaborCost"] + "," +
                                    r["PartCost_AvgBurdenCost"] + "," + r["PartCost_AvgMaterialCost"] + "," +
                                    r["PartCost_AvgSubContCost"] + "," + r["PartCost_AvgMtlBurCost"] + "," + r["Calculated_AVGUnitCost"] + "," + r["PartCost_StdLaborCost"] + "," +
                                    r["PartCost_StdBurdenCost"] + "," + r["PartCost_StdMaterialCost"] + "," + r["PartCost_StdSubContCost"] + "," + r["PartCost_StdMtlBurCost"] + ", " +
                                    r["Part_NonStock"] + "," + r["Part_TypeCode"] + "," + r["Calculated_StdUnitCost"] + "," + r["Part_CreatedOn"] + "," + r["Part_ChangedOn"];

                                comando2.Connection.Open();
                                comando2.Parameters.Clear();
                                comando2.Parameters.AddWithValue("@part_num", r["Part_PartNum"]);
                                comando2.Parameters.AddWithValue("@record", records);
                                comando2.Parameters.AddWithValue("@name_table", "PART_COST");
                                comando2.Parameters.AddWithValue("@error", ex.Message);
                                comando2.Parameters.AddWithValue("@run_at", DateTime.Now);
                                comando2.Parameters.AddWithValue("@query", sqltxt);
                                comando2.ExecuteNonQuery();
                                comando2.Connection.Close();

                            }

                            catch (Exception ex2)
                            {
                                comando2.Connection.Close();
                                ErrorLog.SaveFile("Part Cost", ex2);
                                ErrorLog.SendMail("Part Cost", ex2);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.SaveFile("Part Cost", ex);
                        ErrorLog.SendMail("Part Cost", ex);
                    }
                }
                UpdateDate.updateJobs(4);
                SuccessfulLog.SaveFile("Part cost");
                //

            }
            catch (Exception ex)
            {
                ErrorLog.SaveFile("Part Cost", ex);
                ErrorLog.SendMail("Part Cost", ex);
            }

        }
    }
}
