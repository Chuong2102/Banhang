using SV20T1080026.DomainModels;

namespace SV20T1080026.Web.Models
{
    public class PaginationSearchCustomer : PaginationSearchBaseResult
    {
        public IList<Customer>? Data { get; set; }
    }
}
