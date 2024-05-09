using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom
{
    public class DB
    {
        private static string connectionString = "Data Source=HARISH\\SQLEXPRESS;Initial Catalog=ECOMMERCE;Integrated Security=True";

        public static SqlConnection GetDBConn()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                //Console.WriteLine("Connected to database successfully.");
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to database: " + ex.Message);
            }
            return connection;
        }
    }
}
