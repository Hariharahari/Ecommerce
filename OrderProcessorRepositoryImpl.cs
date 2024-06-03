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
        private static Infofetch i=new Infofetch();
        public bool AddToCart(int cartid, int quantity,int res,int res1)
        {
           
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

        public bool CreateProduct(CartItem product)
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
            try
            {
                using (SqlConnection connection = DB.GetDBConn())
                {
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        
                        string deleteCartQuery = "DELETE FROM cart WHERE product_id = @ProductId";
                        string deleteProductQuery = "DELETE FROM products WHERE product_id = @ProductId";

                        SqlCommand deleteCartCommand = new SqlCommand(deleteCartQuery, connection, transaction);
                        SqlCommand deleteProductCommand = new SqlCommand(deleteProductQuery, connection, transaction);

                        deleteCartCommand.Parameters.AddWithValue("@ProductId", productId);
                        deleteProductCommand.Parameters.AddWithValue("@ProductId", productId);

                        try
                        {
                           
                            deleteCartCommand.ExecuteNonQuery();
                            int rowsAffected = deleteProductCommand.ExecuteNonQuery();

                     
                            if (rowsAffected > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting product: " + ex.Message);
                return false;
            }
        }


        public List<CartItem> GetAllFromCart(int customerId)
        {
            List<CartItem> cartItems = new List<CartItem>();

            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = "SELECT cart_id, product_id, quantity " +
                               "FROM cart " +
                               "WHERE customer_id = @CustomerId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);

                try
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        CartItem cartItem = new CartItem
                        {
                            CartId = reader.GetInt32(reader.GetOrdinal("cart_id")),
                            Product_Id = reader.GetInt32(reader.GetOrdinal("product_id")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity"))
                        };

                        cartItems.Add(cartItem);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error fetching cart items: " + ex.Message);
                }
            }

            return cartItems;
        }

        public bool PlaceOrder(int customer_id, string shippingAddress, int pid, int quantity)
        {
            
            i.ReduceProductStock(pid, quantity);

            if (i.GetProductStockQuantity(pid) >= quantity)
            {
                try
                {
                    using (SqlConnection connection = DB.GetDBConn())
                    {
                      
                        int productPrice = i.GetProductPrice(pid);

                        
                        int totalPrice = quantity * productPrice;

                        
                        string query = "INSERT INTO orders (customer_id, order_date, shipping_address, total_price) " +
                                       "VALUES (@CustomerId, @OrderDate, @ShippingAddress, @TotalPrice)";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@CustomerId", customer_id);
                        command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        command.Parameters.AddWithValue("@ShippingAddress", shippingAddress);
                        command.Parameters.AddWithValue("@TotalPrice", totalPrice);

                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error inserting order: " + ex.Message);
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Out Of Stock!!!");
                return false;
            }
        }

        public bool RemoveFromCart(int customerId)
        {
            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = "DELETE FROM cart WHERE customer_id = @CustomerId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);

                try
                {
                    
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

        public List<Order> GetOrdersByCustomer(int customer_Id)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = DB.GetDBConn())
            {
                string query = @"
                SELECT o.Order_Id, o.Customer_Id, o.Order_Date, o.Shipping_Address, o.Total_Price
                FROM Orders o
                WHERE o.Customer_Id = @CustomerId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customer_Id);

                try
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Order order = new Order();
                        order.OrderId = reader.GetInt32(0);
                        order.CustomerId = reader.GetInt32(1);
                        order.OrderDate = reader.GetDateTime(2);
                        order.ShippingAddress = reader.GetString(3);
                        order.Total_Price = reader.GetInt32(4); 
                        orders.Add(order);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error fetching orders: " + ex.Message);
                }
            }

            return orders;
        }


    }
}
