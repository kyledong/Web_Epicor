using System;
using System.Data.SqlClient;
using Web_Epicor.Data.ConnectionSQL;
using System.Data;
using System.Diagnostics;
using Web_Epicor.Results;


namespace Web_Epicor.Data.Procedures
{
    public class LoadEmails
    {
        public static string Emails()
        {
            string email = "";
            string emails = "";

            SqlConnection sqlCon = new SqlConnection();

            try
            {
                sqlCon = ConnectionToSql.getInstancia().CreateConnection();
                SqlCommand command = new SqlCommand("users", sqlCon);
                command.CommandType = CommandType.StoredProcedure;
                sqlCon.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    email = reader["email"].ToString();
                    emails += email + ", ";

                }
                sqlCon.Close();

                emails = emails.Remove(emails.Length - 2);
            }
            catch (Exception ex)
            {
                ErrorLog.SaveFile("Cargar correos", ex);

                Debug.WriteLine(ex.Message);
            }

            return emails;
        }
    }
}
