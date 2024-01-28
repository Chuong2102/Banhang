using SV20T1080026.DomainModels;

namespace SV20T1080026.Web.Models
{
    public class PaginationSearchCategory : PaginationSearchBaseResult
    {
        public IList<Category> Data { get; set; }
    }
}
