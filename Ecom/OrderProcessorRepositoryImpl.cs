using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom
{
    public class OrderProcessorRepositoryImpl : IOrderProcessorRepository
    {
        public bool AddToCart(int cartid, int quantity)
        {
            //fetch a customer id
            Console.WriteLine("Enter Your password: ");
            string pass = Console.ReadLine();
            Infofetch fetch = new Infofetch();
            int res = fetch.GetCustomerIdByPassword(pass);

            //fetch product id using product name
            Console.WriteLine("Enter a Product name: ");
            string Pname = Console.ReadLine();
            int res1 = fetch.GetProductIdByName(Pname);
            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = "INSERT INTO cart (cart_id, customer_id, product_id, quantity) " +
                               "VALUES (@CartId, @CustomerId, @ProductId, @Quantity)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CartId", cartid);
                command.Parameters.AddWithValue("@CustomerId", res);
                command.Parameters.AddWithValue("@ProductId", res1);
                command.Parameters.AddWithValue("@Quantity", quantity);

                try
                {
                    //connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Item added to cart successfully.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Failed to add item to cart.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception, log error, etc.
                    Console.WriteLine("Error adding to cart: " + ex.Message);
                    return false;
                }
            }
        }

        public bool CreateCustomer(Customer customer)
        {
            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = "INSERT INTO customers (Customer_ID,name, email, password) " +
                               "VALUES (@Customer_ID,@Name, @Email, @Password)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Customer_ID", customer.Customer_Id);
                command.Parameters.AddWithValue("@Name", customer.Name);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@Password", customer.Password);
                try
                {
                    //connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error in inserting a data:"+ ex.Message);
                    return false;       
                }
                
            }
        }

        public bool CreateProduct(Product product)
        {
            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = "INSERT INTO products (product_id, name, price, description, stockQuantity) " +
                               "VALUES (@ProductId, @Name, @Price, @Description, @StockQuantity)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductId", product.Product_Id);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);

                try
                {
                    //connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error inserting product: " + ex.Message);
                    return false;
                }
            }
        }

        public bool DeleteCustomer(int customerId)
        {
            try
            {
                using (SqlConnection connection = DB.GetDBConn())
                {
                    string deleteOrdersQuery = "DELETE FROM orders WHERE customer_id = @CustomerId";
                    SqlCommand deleteOrdersCommand = new SqlCommand(deleteOrdersQuery, connection);
                    deleteOrdersCommand.Parameters.AddWithValue("@CustomerId", customerId);
                    deleteOrdersCommand.ExecuteNonQuery();

                    string deleteCartQuery = "DELETE FROM cart WHERE customer_id = @CustomerId";
                    SqlCommand deleteCartCommand = new SqlCommand(deleteCartQuery, connection);
                    deleteCartCommand.Parameters.AddWithValue("@CustomerId", customerId); 
                    
                    deleteCartCommand.ExecuteNonQuery(); 

                    string deleteCustomerQuery = "DELETE FROM customers WHERE customer_id = @CustomerId";
                    SqlCommand deleteCustomerCommand = new SqlCommand(deleteCustomerQuery, connection);
                    deleteCustomerCommand.Parameters.AddWithValue("@CustomerId", customerId); 
                    int rowsAffected = deleteCustomerCommand.ExecuteNonQuery();

                    return rowsAffected > 0; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting customer: " + ex.Message);
                return false;
            }
        }

        public bool DeleteProduct(int productId)
        {
            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = "DELETE FROM products WHERE product_id = @ProductId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductId", productId);

                try
                {
                    //connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine("Error deleting product: " + ex.Message);
                    return false;
                }
            }
        }

        public List<Product> GetAllFromCart(int customer_Id)
        {
            List<Product> cartProducts = new List<Product>();

            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = "SELECT p.* " +
                               "FROM products p " +
                               "INNER JOIN cart c ON p.product_id = c.product_id " +
                               "WHERE c.customer_id = @CustomerId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customer_Id);

                try
                {
                    //connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        
                        Product product = new Product
                        {
                            Product_Id = reader.GetInt32(reader.GetOrdinal("product_id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Price = reader.GetDecimal(reader.GetOrdinal("price")),
                            Description = reader.GetString(reader.GetOrdinal("description")),
                            StockQuantity = reader.GetInt32(reader.GetOrdinal("stockQuantity"))
                            
                        };

                        
                        cartProducts.Add(product);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    // Handle exception, log error, etc.
                    Console.WriteLine("Error fetching cart products: " + ex.Message);
                }
            }

            return cartProducts;
        
    }

        public bool PlaceOrder(int order_id, int customer_id, string shippingAddress)
        {
            try
            {
                using (SqlConnection connection = DB.GetDBConn())
                {
                    string query = "INSERT INTO orders (order_id, customer_id, order_date, shipping_address) " +
                                   "VALUES (@OrderId, @CustomerId, @OrderDate, @ShippingAddress)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@OrderId", order_id); 
                    command.Parameters.AddWithValue("@CustomerId", customer_id); 
                    command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                    command.Parameters.AddWithValue("@ShippingAddress", shippingAddress);

                    //connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle exception, log error, etc.
                Console.WriteLine("Error inserting order: " + ex.Message);
                return false;
            }

        }

        public bool RemoveFromCart(int cartid)
        {
            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = "DELETE FROM cart WHERE cart_id = @CartId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CartId", cartid);

                try
                {
                    //onnection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine("Error removing from cart: " + ex.Message);
                    return false;
                }
            }
        }
    }
}
