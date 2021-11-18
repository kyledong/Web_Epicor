using System;
using System.Data.SqlClient;
using Web_Epicor.Data.AppJson;

namespace Web_Epicor.Data.ConnectionSQL
{
    public class ConnectionToSql
    {
        private static ConnectionToSql con = null;

        private ConnectionToSql()
        { }

        public SqlConnection CreateConnection()
        {
            SqlConnection Cadena = new SqlConnection();
            try
            {

               // Cadena.ConnectionString = "Server = AT1LDFONSECA\\SQLEXPRESS ; DataBase = Epicor_BI; integrated security = true"; // JSON
                Cadena.ConnectionString = LoadJsonData.ConnetionString(); // JSON
                

            }
            catch (Exception ex)
            {

                Cadena = null;
                throw ex;
            }
            return Cadena;
        }
        public static ConnectionToSql getInstancia()
        {
            if (con == null)
            {
                con = new ConnectionToSql();

            }
            return con;

        }
    }
}
