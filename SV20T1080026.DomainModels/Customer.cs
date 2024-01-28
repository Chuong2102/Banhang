using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080026.DomainModels
{
    /// <summary>
    /// 
    /// </summary>
    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; } = "";
        public string ContactName { get; set; } = "";
        public string Province { get; set; } = "";
        public string Address { get; set; } = "";
        public string? Phone { get; set; }
        [StringLength(50)]
        public string? Email { get; set; }
        public bool? IsLocked { get; set; }
    }
}
