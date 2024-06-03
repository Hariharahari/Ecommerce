using System;

using System.Data.SqlClient;


namespace LoanManagementSystem
{
    public class DBUtil
    {
        private static string connectionString = "Data Source=HARISH\\SQLEXPRESS;Initial Catalog=ECOMMERCE;Integrated Security=True;Encrypt=True";

        public static SqlConnection GetDBConn()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                Console.WriteLine("Connected to database successfully.");
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to database: " + ex.Message);
            }
            return connection;
        }
    }
}
