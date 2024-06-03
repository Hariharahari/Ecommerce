using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom
{
    public interface IOrderProcessorRepository
    {
        bool CreateProduct(CartItem product);
        bool CreateCustomer(Customer customer);
        bool DeleteProduct(int productId);
        bool DeleteCustomer(int customerId);
        bool AddToCart(int cartid, int quantity,int res,int res1);
        bool RemoveFromCart(int cartid);
        List<CartItem> GetAllFromCart(int customer_id);
        bool PlaceOrder(int customer_id, string shippingAddress,int pid,int quantity);
        List<Order> GetOrdersByCustomer(int customer_id);
    }
}
