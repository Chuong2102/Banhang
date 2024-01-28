using SV20T1080026.DomainModels;

namespace SV20T1080026.Web.Models
{
    public class PaginationSearchProduct : PaginationSearchBaseResult
    {
        public IList<Product>? Data { get; set; }
        public int EmployeeID { get; set; }
        public int CustomerID { get; set; }
    }
}
