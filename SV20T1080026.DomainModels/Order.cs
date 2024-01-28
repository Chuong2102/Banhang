using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080026.DomainModels
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderTime { get; set; }
        public string DeliveryProvince { get; set; } = "";
        public string DeliveryAddress { get; set; } = "";
        public int EmployeeID { get; set; }
        public DateTime AcceptTime { get; set; }
        public int ShipperID { get; set; }
        public DateTime ShippedTime { get; set;}
        public DateTime FinishedTime { get; set; }
        public int Status { get; set; }
    }

    public class OrderDetail
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
    }

    public class OrderStatus
    {
        public int Status { get; set; }
        public string Description { get; set; } = "";
    }
}
