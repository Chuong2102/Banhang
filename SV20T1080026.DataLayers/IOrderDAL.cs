using SV20T1080026.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080026.DataLayers
{
    /// <summary>
    /// Định nghĩa các phép xử lý dữ liệu liên quan đến đơn hàng
    /// </summar
    public interface IOrderDAL
    {
        IList<Order> List(int page = 1, int pageSize = 0, string searchValue = "", int status = 0);
        int Add(Order order);
        bool Update(Order order);
        Order? GetById(int id);
        bool Delete(int orderID);
        int Count(string searchValue, int status);
        long AddOrderDetail(OrderDetail orderDetail);
        OrderStatus? GetOrderStatus(int orderStatusID);
        IList<OrderStatus> ListOrderStatus();
        IList<OrderDetail> ListOrderDetail(int orderID);
        OrderDetail GetOrderDetail(int orderID, int productID);
        bool UpdateOrderDetail(OrderDetail orderDetail);
        bool DeleteOrderDetail(int orderID, int productID);
        bool DeleteOrderDetail(int orderID);
        bool DeleteOrderDetailByProductID(int productID);
    }
}
