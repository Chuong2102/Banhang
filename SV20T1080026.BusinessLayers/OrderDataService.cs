using SV20T1080026.DataLayers;
using SV20T1080026.DataLayers.SQLServer;
using SV20T1080026.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080026.BusinessLayers
{
    public static class OrderDataService
    {
        static readonly IOrderDAL orderDB;

        static OrderDataService()
        {
            string connectionString = "server=LAPTOP-HP5KFASE\\SQLEXPRESS;user id=sa;password=1;database=LiteCommerceDB;TrustServerCertificate=true";
            orderDB = new OrderDAL(connectionString);
        }

        public static int Add(Order order)
        {
            return orderDB.Add(order);
        }

        public static bool Update(Order order)
        {
            return orderDB.Update(order);
        }

        public static bool Delete(int orderID)
        {
            return orderDB.Delete(orderID);
        }

        public static Order? GetByID(int id)
        {
            return orderDB.GetById(id);
        }

        public static long AddOrderDetail(OrderDetail orderDetail)
        {
            return orderDB.AddOrderDetail(orderDetail);
        }

        public static List<Order> List(out int rowCount ,int page = 1, int pageSize = 0, string searchValue = "", int status = 0)
        {
            rowCount = orderDB.Count(searchValue, status);
            return orderDB.List(page, pageSize, searchValue, status).ToList();
        }

        public static List<OrderStatus> ListOrderStatuses()
        {
            return orderDB.ListOrderStatus().ToList();
        }

        public static List<OrderDetail> ListOrderDetails(int orderID)
        {
            return orderDB.ListOrderDetail(orderID).ToList();
        }

        public static OrderDetail GetOrderDetail(int orderID, int productID)
        {
            return orderDB.GetOrderDetail(orderID, productID);
        }

        public static OrderStatus? GetOrderStatus(int status)
        {
            return orderDB.GetOrderStatus(status);
        }

        public static bool UpdateOrderDetail(OrderDetail orderDetail)
        {
            return orderDB.UpdateOrderDetail(orderDetail);
        }

        public static bool DeleteOrderDetail(int orderID, int productID)
        {
            return orderDB.DeleteOrderDetail(orderID, productID);
        }

        public static bool DeleteOrderDetail(int orderID)
        {
            return orderDB.DeleteOrderDetail(orderID);
        }
    }
}
