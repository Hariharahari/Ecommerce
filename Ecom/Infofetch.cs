using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom
{
    public class Infofetch
    {
        public int GetCustomerIdByPassword(string password)
        {
            int customerId = -1;

            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = "SELECT customer_id FROM customers WHERE password = @Password";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Password", password);

                try
                {
                    //connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        customerId = reader.GetInt32(0);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error fetching customer ID: " + ex.Message);
                }
            }

            return customerId;
        }
        public int GetProductIdByName(string productName)
        {
            int productId = -1;

            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = "SELECT product_id FROM products WHERE name = @ProductName";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductName", productName);

                try
                {
                    //connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        productId = reader.GetInt32(0);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error fetching product ID: " + ex.Message);
                }
            }

            return productId;
        }
    }
}
