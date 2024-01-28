using SV20T1080026.DomainModels;

namespace SV20T1080026.Web.Models
{
    public class OrderItem
    {
        public Order? Order { get; set; }
        public Employee? Employee { get; set; }
        public Customer? Customer { get; set; }
        List<Product>? Products { get; set; }
    }
}
