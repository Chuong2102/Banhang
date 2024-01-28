using SV20T1080026.DomainModels;

namespace SV20T1080026.Web.Models
{
    public class PaginationSearchSupplier : PaginationSearchBaseResult
    {
        public IList<Supplier>? Data { get; set; }
    }
}
