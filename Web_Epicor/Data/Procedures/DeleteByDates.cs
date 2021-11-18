using System;
using System.Data;
using System.Data.SqlClient;
using Web_Epicor.Data.ConnectionSQL;
using Web_Epicor.Results;

namespace Web_Epicor.Data.Procedures
{
    public class DeleteByDates
    {

        public static string DeleteByDatesLaborTime(DateTime startDate, DateTime finalDate)
        {

            SqlConnection sqlCon = new SqlConnection();
            string rpta = "";

            try
            {
                sqlCon = ConnectionToSql.getInstancia().CreateConnection();
                SqlCommand command = new SqlCommand("deleteByDates", sqlCon);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("start_date", SqlDbType.Date).Value = startDate;
                command.Parameters.AddWithValue("final_date", SqlDbType.Date).Value = finalDate;
                sqlCon.Open();
                
                 rpta = command.ExecuteNonQuery() >= 0? "OK" : "No se pudo eliminar por fechas en Labor Time.";
            }
            catch(Exception ex)
            {
                ErrorLog.SaveFile("Eliminar por fechas en Labor Time", ex);
                ErrorLog.SendMail("Eliminar por fechas en Labor Time", ex);
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open) sqlCon.Close();
            }
            return rpta;
            
        }

    }
}
