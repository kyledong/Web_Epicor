using System;
using System.Data;
using System.Data.SqlClient;
using Web_Epicor.Data.ConnectionSQL;
using Web_Epicor.Results;

namespace Web_Epicor.Data.Procedures
{
    public class UpdateDate
    {
        public static string updateJobs(int id)
        {
            string rpta = "";
            SqlConnection sqlCon = new SqlConnection();

            try
            {
                sqlCon = ConnectionToSql.getInstancia().CreateConnection();
                SqlCommand command = new SqlCommand("update_jobs", sqlCon);
                command.CommandType = CommandType.StoredProcedure;
                // command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                command.Parameters.AddWithValue("@id", id);
                sqlCon.Open();
                rpta = command.ExecuteNonQuery() == 1 ? "OK" : "No se pudo actualizar el registro.";
            }
            catch (Exception ex)
            {
                ErrorLog.SaveFile("Actualizar fecha de registro", ex);
                ErrorLog.SendMail("Actualizar fecha de registro", ex);
              
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open) sqlCon.Close();

            }
            return rpta;
           
        }
    }
}
