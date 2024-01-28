namespace SV20T1080026.Web.Models
{
    public class PaginationSearchInput
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchValue { get; set; } = "";
        public int EmployeeID { get; set; }
        public int CustomerID { get; set; }
    }
}
