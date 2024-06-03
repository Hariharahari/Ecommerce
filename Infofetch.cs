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
        public int GetProductStockQuantity(int productId)
        {
            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = "SELECT stockquantity FROM products WHERE product_id = @ProductId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductId", productId);

                try
                {
                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int stockQuantity))
                    {
                        return stockQuantity;
                    }
                    else
                    {
                        throw new Exception("Product not found or invalid stock quantity.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error fetching stock quantity: " + ex.Message);
                    throw;
                }
            }
        }
        public bool ReduceProductStock(int productId, int quantityOrdered)
        {
            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = "UPDATE products SET stockquantity = stockquantity - @QuantityOrdered WHERE product_id = @ProductId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@QuantityOrdered", quantityOrdered);
                command.Parameters.AddWithValue("@ProductId", productId);

                try
                {

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reducing stock quantity: " + ex.Message);
                    return false;
                }
            }
        }
        public int GetProductPrice(int productId)
        {
            using (SqlConnection connection = DB.GetDBConn())
            {

                string query = "SELECT price FROM products WHERE product_id = @ProductId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        throw new Exception("Product not found");
                    }
                }
            }
        }
        public int GetCartIdByCustomerId(int customerId)
        {
            int cartId = -1;

            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = "SELECT cart_id FROM cart WHERE customer_id = @CustomerId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);

                try
                {

                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        cartId = Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error fetching cart ID: " + ex.Message);
                }
            }

            return cartId;
        }
        public bool IsProductInCart(int cartId, int productId)
        {
            bool isInCart = false;

            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = "SELECT COUNT(*) FROM cart WHERE cart_id = @CartId AND product_id = @ProductId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CartId", cartId);
                command.Parameters.AddWithValue("@ProductId", productId);

                try
                {

                    int count = (int)command.ExecuteScalar();
                    isInCart = count > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error checking if product is in cart: " + ex.Message);
                }
            }

            return isInCart;
        }
        public bool AddQuantityToCart(int cartId, int productId, int quantityToAdd)
        {
            try
            {
                using (SqlConnection connection = DB.GetDBConn())
                {
                   
                    string checkQuery = "SELECT quantity FROM cart WHERE cart_id = @CartId AND product_id = @ProductId";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@CartId", cartId);
                    checkCommand.Parameters.AddWithValue("@ProductId", productId);

                    object result = checkCommand.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                       
                        int currentQuantity = Convert.ToInt32(result);
                        int newQuantity = currentQuantity + quantityToAdd;

                        string updateQuery = "UPDATE cart SET quantity = @NewQuantity WHERE cart_id = @CartId AND product_id = @ProductId";
                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@NewQuantity", newQuantity);
                        updateCommand.Parameters.AddWithValue("@CartId", cartId);
                        updateCommand.Parameters.AddWithValue("@ProductId", productId);

                        updateCommand.ExecuteNonQuery();
                    }
                   
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding quantity to cart: " + ex.Message);
                return false;
            }
        }
        public bool DeleteCartItem(int cartId, int productId)
        {
            try
            {
                using (SqlConnection connection = DB.GetDBConn())
                {
                    string query = "DELETE FROM cart WHERE cart_id = @CartId AND product_id = @ProductId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CartId", cartId);
                    command.Parameters.AddWithValue("@ProductId", productId);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting cart item: " + ex.Message);
                return false;
            }
        }
        public bool AddOrUpdateProduct(string productName, int productPrice, int quantity)
        {
            using (SqlConnection connection = DB.GetDBConn())
            {
                try
                {
                    string checkQuery = "SELECT product_id, stockquantity FROM products WHERE name = @ProductName AND price = @ProductPrice";

                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@ProductName", productName);
                    checkCommand.Parameters.AddWithValue("@ProductPrice", productPrice);

                    SqlDataReader reader = checkCommand.ExecuteReader();

                    if (reader.Read())
                    {
                        int productId = reader.GetInt32(reader.GetOrdinal("product_id"));
                        int existingStockQuantity = reader.GetInt32(reader.GetOrdinal("stockquantity"));
                        int newStockQuantity = existingStockQuantity + quantity;

                        reader.Close();

                        string updateQuery = "UPDATE products SET stockquantity = @NewStockQuantity WHERE product_id = @ProductId";
                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@NewStockQuantity", newStockQuantity);
                        updateCommand.Parameters.AddWithValue("@ProductId", productId);

                        updateCommand.ExecuteNonQuery();
                    }
                    

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);  
                    return false;
                }
            }
        }

    }
}


