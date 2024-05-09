using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Console.WriteLine("7. Remove Item From Cart");
            Console.WriteLine("8. Place Order");
            Console.WriteLine("9. Exit");
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
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
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
            Console.WriteLine("Enter a product id:");
            int pid=int.Parse(Console.ReadLine());
            Console.WriteLine("Enter a Product name:");
            string name=Console.ReadLine();
            Console.WriteLine("Enter a price");
            int price=int.Parse(Console.ReadLine());
            Console.WriteLine("Enter a description:");
            string desc=Console.ReadLine();
            Console.WriteLine("Enter a Stock Quantity");
            int sq=int.Parse(Console.ReadLine());
            Product product = new Product() { Product_Id=pid,Name=name,Price=price,Description=desc,StockQuantity=sq};
            if(orderProcessorRepository.CreateProduct(product))
            {
                Console.WriteLine("Product is added sucessfully");
            }
            else
            {
                Console.WriteLine("Product is not Added:");
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
            Console.WriteLine("Enter your product to a cart.....");
            Console.Write("Enter a Cart id: ");
            int cid = int.Parse(Console.ReadLine());
            Console.Write("Enter a Quantity: ");
            int q=int.Parse(Console.ReadLine());
            
            Customer customer = new Customer();
            Product product = new Product();
            if(orderProcessorRepository.AddToCart(cid,q)) 
            { 
                Console.WriteLine("The Product is added sucessfully");
            }
            else
            {
                Console.WriteLine("Adding data to thje cart is unsucessful");
            }
            
        }
        private static void ViewCart() 
        {
            Console.WriteLine("Enter a Customer ID to view a cart.....");
            int c= int.Parse(Console.ReadLine());
            List<Product> cartProducts = orderProcessorRepository.GetAllFromCart(c);

            // Display the details of the products in the cart
            if (cartProducts.Count > 0)
            {
                Console.WriteLine("Products in the cart for customer ID " + c + ":");
                foreach (Product product in cartProducts)
                {
                    Console.WriteLine("Product ID: " + product.Product_Id);
                    Console.WriteLine("Name: " + product.Name);
                    Console.WriteLine("Price: " + product.Price);
                    Console.WriteLine("Description: " + product.Description);
                    Console.WriteLine("Stock Quantity: " + product.StockQuantity);
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No products found in the cart for customer ID " + c + ".");
            }
        }
        private static void RemoveFromCart()
        {
            Console.WriteLine("Enter a Cart id that you want to remove....");
            int cartid= int.Parse(Console.ReadLine());
            if (orderProcessorRepository.RemoveFromCart(cartid))
            {
                Console.WriteLine("The Item is removed from the cart successfully.....");
            }
            else
            {
                Console.WriteLine("Sorry!!! There is a problem in deleting a Cart Item");
            }
            
        }
        private static void PlaceOrder()
        {
            Console.WriteLine("Enter a password...");
            string pass= Console.ReadLine();
            Infofetch infofetch = new Infofetch();
            int res=infofetch.GetCustomerIdByPassword(pass);
            Console.WriteLine("Enter a shipping Address");
            string add= Console.ReadLine();
            Console.WriteLine("Enter a Order ID:");
            int oid=int.Parse(Console.ReadLine());
            if (orderProcessorRepository.PlaceOrder(oid,res, add))
            {
                Console.WriteLine("Congrats!!! Your order is placed sucessfully:");
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


    }
}
