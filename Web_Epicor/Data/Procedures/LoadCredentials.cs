using System;
using System.Data.SqlClient;
using Web_Epicor.Data.ConnectionSQL;
using System.Data;
using System.Diagnostics;
using Web_Epicor.Results;

namespace Web_Epicor.Data.Procedures
{
    public class LoadCredentials
    {
        //Será con la del correo
        
       
        public static string Email(){

            string email = "";
           
            SqlConnection sqlCon = new SqlConnection();
            
         
            try
            {
                sqlCon = ConnectionToSql.getInstancia().CreateConnection();
                SqlCommand command = new SqlCommand("load_user", sqlCon);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", 1);
                sqlCon.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    
                    email = reader["email"].ToString();
                    
                }
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                ErrorLog.SaveFile("Cargar email para credenciales", ex);
                
                Debug.WriteLine(ex.Message);
            }

            return email;

        }

        public static string Password()
        {

            string password = "";

            SqlConnection sqlCon = new SqlConnection();

            try
            {
                sqlCon = ConnectionToSql.getInstancia().CreateConnection();
                SqlCommand command = new SqlCommand("load_user", sqlCon);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", 1);
                sqlCon.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {

                    password = reader["pass"].ToString();

                }
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                ErrorLog.SaveFile("Cargar password para credenciales", ex);
                ErrorLog.SendMail("Cargar password para credenciales", ex);

                Debug.WriteLine(ex.Message);
            }

            return password;

        }



    }
}
