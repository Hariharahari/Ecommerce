using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ecom
{
    internal class Program
    {
        private static IOrderProcessorRepository orderProcessorRepository=new OrderProcessorRepositoryImpl();
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Ecommerce website:");
            Console.WriteLine("1. Register Customer.");
            Console.WriteLine("2. Create Product.");
            Console.WriteLine("3. Delete Product.");
            Console.WriteLine("4. Delete Customer.");
            Console.WriteLine("5. Add to cart.");
            Console.WriteLine("6. View cart.");
            Console.WriteLine("7. Remove Customer's Cart");
            Console.WriteLine("8. Place Order");
            Console.WriteLine("9. Order details of a customer:");
            Console.WriteLine("10. Exit");
            while (true)
            {
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddCustomer();
                        break;
                    case "2":
                        AddProduct();
                        break;
                    case "3":
                        DeleteProduct();
                        break;
                    case "4":
                        DeleteCustomer();
                        break;
                    case "5":
                        Addcart();
                        break;
                    case "6":
                        ViewCart();
                        break;
                        
                    case "7":
                        RemoveFromCart();
                        break;
                    case "8":
                        PlaceOrder();
                        break;
                    case "9":
                        Orderdetails();
                        break;
                    case "10":
                        Console.WriteLine("Exiting...");
                        return;
                    case "11":
                        fetchstock();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void RemoveFromCart()
        {
            Console.WriteLine("Enter a Customer id to remove a cart:");
            int c=int.Parse(Console.ReadLine());
            if (orderProcessorRepository.RemoveFromCart(c))
            {
                Console.WriteLine("The Cart is Removed sucessfully:");

            }
            else
            {
                Console.WriteLine("The Cart is not deleted or there is no cart for customer:");
            }
        }

        private static void AddCustomer()
        {
            Console.WriteLine("Enter the customer id:  ");
            int id=int.Parse(Console.ReadLine());
            Console.WriteLine("Enter a Name:");
            string name=Console.ReadLine();
            Console.WriteLine("Enter a email address:");
            string email=Console.ReadLine();
            Console.WriteLine("Enter a password:");
            string pass=Console.ReadLine();
            Customer customer=new Customer() { Customer_Id=id,Name=name,Email=email,Password=pass};
            bool res= orderProcessorRepository.CreateCustomer(customer);
            if (res)
            {
                Console.WriteLine("Customer detail created sucessfully:");
            }
            else
            {
                Console.WriteLine("Deatails not added to a database:");
            }
        }
        private static void AddProduct()
        {
            Console.WriteLine("Enter a Product name: ");
            string name=Console.ReadLine();
            Console.WriteLine("Enter a price");
            int price=int.Parse(Console.ReadLine());
            Console.WriteLine("Enter a description:");
            string desc=Console.ReadLine();
            Console.WriteLine("Enter a Stock Quantity");
            int sq=int.Parse(Console.ReadLine());
            Infofetch infofetch=new Infofetch();
            if (infofetch.AddOrUpdateProduct(name, price, sq))
            {
                Console.WriteLine("The Product is already in our App Its  Quantity is updated sucessfully");
            }
            else {
                Console.WriteLine("Enter a Product id: ");
                int pid=int.Parse(Console.ReadLine());
                CartItem product = new CartItem() { Product_Id=pid,Name = name, Price = price, Description = desc, StockQuantity = sq };
                if (orderProcessorRepository.CreateProduct(product))
                {
                    Console.WriteLine("Product is added sucessfully");
                }
                else
                {
                    Console.WriteLine("Product is not Added:");
                }
            }
        }
        private static void DeleteProduct()
        {
            Console.Write("Enter a Product Id to delete:");
            int pid= int.Parse(Console.ReadLine());
            if(orderProcessorRepository.DeleteProduct(pid)) 
            { 
                Console.WriteLine("The Product is deleted sucessfully:"); 
            }
        }
        private static void Addcart()
        {
            Console.WriteLine("Enter Your Customer ID: ");
            int pass = int.Parse(Console.ReadLine());
            Infofetch i=new Infofetch();
            int CartId=i.GetCartIdByCustomerId(pass);
            if (CartId != -1)
            {
                Console.WriteLine("Welcome back");
                Console.WriteLine("Cart ID: " + CartId);
                Console.Write("Enter a Quantity: ");
                int q = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter a product id: ");
                int Pname = int.Parse(Console.ReadLine());
                if (i.IsProductInCart(CartId, Pname))
                {
                    Console.WriteLine("The Product is already in cart");
                    Console.WriteLine("Quantity is Updated");
                    i.AddQuantityToCart(CartId,Pname,q);
                }
                else
                {
                    if (orderProcessorRepository.AddToCart(CartId, q, pass, Pname))
                    {
                        Console.WriteLine("The Product is added sucessfully");
                    }
                    else
                    {
                        Console.WriteLine("Adding data to the cart is unsucessful");
                    }
                }
                
            }
            else
            {
                Console.Write("Enter a Cart id: ");
                int cid = int.Parse(Console.ReadLine());
                Console.Write("Enter a Quantity: ");
                int q = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter a product id: ");
                int Pname = int.Parse(Console.ReadLine());
                
                if (orderProcessorRepository.AddToCart(cid, q, pass, Pname))
                {
                    Console.WriteLine("The Product is added sucessfully");
                }
                else
                {
                    Console.WriteLine("Adding data to the cart is unsucessful");
                }
            }
            
            


            
            
        }
        private static void ViewCart()
        {
            Console.WriteLine("Enter a Customer ID to view the cart:");
            int customerId;
            if (int.TryParse(Console.ReadLine(), out customerId))
            {
                List<CartItem> cartItems = orderProcessorRepository.GetAllFromCart(customerId);

                if (cartItems.Count > 0)
                {
                    Console.WriteLine("Products in the cart for customer ID " + customerId + ":");
                    foreach (var item in cartItems)
                    {
                        Console.WriteLine($"\nCart ID: {item.CartId}, \nProduct ID: {item.Product_Id}, \nQuantity: {item.Quantity}");
                    }
                }
                else
                {
                    Console.WriteLine("No products found in the cart for customer ID " + customerId + ".");
                }
            }
            else
            {
                Console.WriteLine("Invalid Customer ID. Please enter a valid number.");
            }
        }

        private static void PlaceOrder()
        {
            Infofetch i = new Infofetch();
            Console.WriteLine("Enter a Customer Id: ");
            int res=int.Parse(Console.ReadLine());
            Console.WriteLine("Enter a shipping Address: ");
            string add= Console.ReadLine();
            Console.WriteLine("Enter a product id: ");
            int pid = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter a Quantity: ");
            int quantity = int.Parse(Console.ReadLine());
            if (orderProcessorRepository.PlaceOrder(res, add,pid,quantity))
            {
                Console.WriteLine(" Your order is placed sucessfully:");
                int CartId = i.GetCartIdByCustomerId(res);
                i.DeleteCartItem(CartId,pid);
            }
            else
            {
                Console.WriteLine("Sorry!! Yourt order is not Placed:");
            }
        }
        private static void DeleteCustomer() 
        {
            Console.WriteLine("Enter a customer id to delete:");
            int cid= int.Parse(Console.ReadLine());
            if(orderProcessorRepository.DeleteCustomer(cid))
            {
                Console.WriteLine("The Customer details Deleted sucessfully");
            }
            else
            {
                Console.WriteLine("The customer detail is not deleted");
            }
        }
        private static void Orderdetails()
        {
            Console.WriteLine("Enter a Customer Id: ");
            int pass= int.Parse(Console.ReadLine());
            OrderProcessorRepositoryImpl orderProcessorRepository = new OrderProcessorRepositoryImpl();
            List<Order> orders = orderProcessorRepository.GetOrdersByCustomer(pass);
            Console.WriteLine("Orders placed by customer:");
            foreach (var order in orders)
            {
                Console.WriteLine($"\nOrder ID: {order.OrderId} \nDate: {order.OrderDate} \nCustomerId: {order.CustomerId} \nShipping Address: {order.ShippingAddress} \nTotal Price: {order.Total_Price}");
            }
        }
        private static void fetchstock()
        {
           
            Infofetch i= new Infofetch();
            int cartId = 102; // Example cart ID
            int productId = 1; // Example product ID
            int quantityToAdd = 2; 

            bool success = i.AddQuantityToCart(cartId, productId, quantityToAdd);

            if (success)
            {
                Console.WriteLine("Quantity added successfully.");
            }
            else
            {
                Console.WriteLine("Failed to add quantity to cart.");
            }

        }


    }
}
