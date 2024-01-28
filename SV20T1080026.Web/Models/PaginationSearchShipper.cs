using SV20T1080026.DomainModels;

namespace SV20T1080026.Web.Models
{
    public class PaginationSearchShipper : PaginationSearchBaseResult
    {
        public IList<Shipper> Data { get; set; }
    }
}
