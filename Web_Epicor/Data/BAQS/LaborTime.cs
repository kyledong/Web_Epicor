using System;
using System.Data;
using System.Data.SqlClient;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using Web_Epicor.Data.Procedures;
using Web_Epicor.Data.AppJson;
using Web_Epicor.Results;
using Web_Epicor.Data.Dates;

namespace Web_Epicor.Data.BAQS
{
    public class LaborTime
    {
        public static void GetLaborTime()
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
                objBulk.DestinationTableName = "Labor_Time";

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
                SuccessfulLog.SaveFile("Labor Time");

            }
            catch (Exception ex)
            {
                ErrorLog.SaveFile("Labor Time", ex);
                ErrorLog.SendMail("Labor Time", ex);
            }
        }

        public static void GetLaborTime(int option)
        {
            

            try
            {
                //Elimino datos del mes anterior en tabla de SQL, filtro la API por Mes e inserto.
                switch (option)
                {
                    case 1: //Mensual


                        var month = CalculateDates.Monthly();
                        DateTime startDate = month.Item1;
                        DateTime finalDate = month.Item2;

                        string result = DeleteByDates.DeleteByDatesLaborTime(startDate, finalDate);

                        if (result.Equals("OK"))
                        {
                            string strStartDate = startDate.ToString("yyyy-MM-dd");
                            string strFinalDate = finalDate.ToString("yyyy-MM-dd");


                            string url = LoadJsonData.LaborTime_url() + "?$filter=LaborDtl_ClockInDate ge datetime\'" +strStartDate + "\' and LaborDtl_ClockInDate le datetime\'" + strFinalDate + "\'";
                            SaveLaborTime(LoadJsonData.LaborTime_url() + "?$filter=LaborDtl_ClockInDate ge datetime\'" + strStartDate + "\' and LaborDtl_ClockInDate le datetime\'" + strFinalDate + "\'");
                        }
                        
                        break;
                    case 2: //Semanal

                        var weekly = CalculateDates.Weekly();
                        DateTime startWeek = weekly.Item1;
                        DateTime finalWeek = weekly.Item2;



                        //Elimina los datos que se filtran de inicio a fin de semana y retorna el resultado.
                        string result2 = DeleteByDates.DeleteByDatesLaborTime(startWeek, finalWeek);
                        if (result2.Equals("OK"))
                        {
                            string strStartWeek = startWeek.ToString("yyyy-MM-dd");
                            string strFinalWeek = finalWeek.ToString("yyyy-MM-dd");
                            //Gurada los datos eliminados anteriormente.
                            string save = SaveLaborTime(LoadJsonData.LaborTime_url() + "?$filter=LaborDtl_ClockInDate ge datetime\'" + strStartWeek + "\' and LaborDtl_ClockInDate le datetime\'" + strFinalWeek + "\'");
                            if (save.Equals("OK"))
                            {
                                InsertLog.Insert("Labor time", "Montlhy", startWeek, finalWeek, "Successful at " + DateTime.Now, "Successful registration.");
                            }
                            else{
                                InsertLog.Insert("Labor time", "Montlhy", startWeek, finalWeek, "Registration failed at " + DateTime.Now, "Data insert failed.");
                            }
                        }
                        else
                        {
                            InsertLog.Insert("Labor time", "Montlhy", startWeek, finalWeek, "Deletion failed at " + DateTime.Now, "Data deletion failed.");
                        }
                        

                        break;
                    case 3: //Diaria
                        break;
                    case 4: //Temporizador                        
                        break;
                    default:

                        break;
                }
            }
            catch(Exception ex)
            {
                ErrorLog.SaveFile("Registros por mensualidad en Labor Time", ex);
                ErrorLog.SendMail("Registros por mensualidad en Labor Time", ex);
            }              
            
        }
        static string SaveLaborTime(string url)
        {
            string result = "";
            try
            {
                //string ejem = LoadJsonData.Job_url() + "?$filter=" + param;
                var client = new RestClient(url);              
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
                objBulk.DestinationTableName = "Labor_Time";

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
                SuccessfulLog.SaveFile("Labor Time");
                result = "OK";
                

            }
            catch (Exception ex)
            {
                ErrorLog.SaveFile("Labor Time", ex);
                ErrorLog.SendMail("Labor Time", ex);
                result = "Not";
            }
            return result;
        }
    }
}
