using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom
{
    public interface IOrderProcessorRepository
    {
        bool CreateProduct(Product product);
        bool CreateCustomer(Customer customer);
        bool DeleteProduct(int productId);
        bool DeleteCustomer(int customerId);
        bool AddToCart(int cartid, int quantity);
        bool RemoveFromCart(int cartid);
        List<Product> GetAllFromCart(int customer_id);
        bool PlaceOrder(int order_id,int customer_id, string shippingAddress);
    }
}
