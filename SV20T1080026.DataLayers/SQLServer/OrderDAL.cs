using Azure;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using SV20T1080026.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080026.DataLayers.SQLServer
{
    public class OrderDAL : _BaseDAL, IOrderDAL
    {
        public OrderDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Order order)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into Orders(CustomerID,OrderTime,DeliveryProvince,DeliveryAddress,EmployeeID,AcceptTime,
                                    ShipperID,ShippedTime,FinishedTime,Status)
                                    values(@CustomerID,getdate(),@DeliveryProvince,@DeliveryAddress,@EmployeeID,@AcceptTime,
                                    @ShipperID,@ShippedTime,@FinishedTime,@Status);
                                    select @@identity;";
                var parameters = new
                {
                    order.CustomerID,
                    order.OrderTime,
                    order.DeliveryProvince,
                    order.DeliveryAddress,
                    order.EmployeeID,
                    order.AcceptTime,
                    order.ShipperID,
                    order.ShippedTime,
                    order.FinishedTime,
                    order.Status
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddOrderDetail(OrderDetail orderDetail)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from OrderDetails where OrderID = @OrderID and ProductID = @ProductID)
                                select -1
                            else
                                begin
                                    insert into OrderDetails(OrderID, ProductID, Quantity, SalePrice)
                                    values(@OrderID, @ProductID, @Quantity, @SalePrice);
                                    select @@identity;
                                end";
                var parameters = new
                {
                    orderDetail.OrderID,
                    orderDetail.ProductID,
                    orderDetail.Quantity,
                    orderDetail.SalePrice
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int status = 0)
        {
            int count = 0;

            if (!searchValue.IsNullOrEmpty())
                searchValue = "%" + searchValue + "%";

            using (SqlConnection connection = OpenConnection())
            {
                var sql = @"
                    select count(*)
                    from Orders as o join Customers as c on c.CustomerID = o.CustomerID
						join OrderStatus as os on os.Status = o.Status
				        join Shippers as s on s.ShipperID = o.ShipperID
                        where (@searchvalue = N'' and @status = 0)
                            or (os.Status = @status and @searchvalue = N'')
                            or (@status = 0 and (c.CustomerName like @searchvalue or s.ShipperName like @searchvalue) )
                            or (os.Status = @status and (c.CustomerName like @searchvalue or s.ShipperName like @searchvalue))";

                var para = new
                {
                    searchValue = searchValue,
                    status = status
                };

                count = connection.ExecuteScalar<int>(sql: sql, param: para, commandType: CommandType.Text);

                connection.Close();
            }

            return count;
        }

        public bool Delete(int orderID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Orders where OrderID = @orderID and not exists(select * from OrderDetails where OrderID = @orderID)";
                var parameters = new { orderID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteOrderDetail(int orderID, int productID)
        {
            //TODO:Xoa choa tao (xoa orderDetail)
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"delete from OrderDetails where OrderID = @orderID and ProductID = @productID";
                var parameters = new { orderID, productID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteOrderDetail(int orderID)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"delete from OrderDetails where OrderID = @orderID";
                var parameters = new { orderID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteOrderDetailByProductID(int productID)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"delete from OrderDetails where ProductID = @productID";
                var parameters = new { productID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Order? GetById(int id)
        {
            Order? data = null;

            using (var connection = OpenConnection())
            {
                var sql = "select * from Orders where OrderID = @id";
                var parameters = new { id = id };
                data = connection.QueryFirstOrDefault<Order>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public OrderDetail GetOrderDetail(int orderID, int productID)
        {
            OrderDetail? data = null;

            using (var connection = OpenConnection())
            {
                var sql = @"select * from OrderDetails where OrderID = @orderID and ProductID = @productID";
                var parameters = new { productID, orderID };
                data = connection.QueryFirstOrDefault<OrderDetail>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public OrderStatus? GetOrderStatus(int orderStatusID)
        {
            OrderStatus? data = null;
            using (var connection = OpenConnection())
            {
                var sql = "select * from OrderStatus where Status = @orderStatusID";
                var parameters = new { orderStatusID = orderStatusID };
                data = connection.QueryFirstOrDefault<OrderStatus>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public IList<Order> List(int page = 1, int pageSize = 0, string searchValue = "", int status = 0)
        {
            List<Order> listOrders;

            if (!searchValue.IsNullOrEmpty())
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"
                    with cte as
                    (
	                    select o.OrderID, o.CustomerID, o.OrderTime, o.DeliveryAddress, o.EmployeeID,
                        o.AcceptTime, o.ShipperID, o.ShippedTime, FinishedTime, o.Status,
                        ROW_NUMBER() over (order by c.CustomerName) as RowNumber
                        from Orders as o join Customers as c on c.CustomerID = o.CustomerID
						join OrderStatus as os on os.Status = o.Status
				        join Shippers as s on s.ShipperID = o.ShipperID
                        where (@searchvalue = N'' and @status = 0)
                            or (os.Status = @status and @searchvalue = N'')
                            or (@status = 0 and (c.CustomerName like @searchvalue or s.ShipperName like @searchvalue) )
                            or (os.Status = @status and (c.CustomerName like @searchvalue or s.ShipperName like @searchvalue))
                    )

                    select * from cte
                    where (@pageSize= 0 and @status = 0)
	                    or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                    order by RowNumber;";

                var parameters = new
                {
                    page,
                    pageSize,
                    searchValue,
                    status
                };

                listOrders = connection.Query<Order>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();

                connection.Close();
            }

            if (listOrders == null)
                listOrders = new List<Order>();

            return listOrders;
        }

        public IList<OrderDetail> ListOrderDetail(int orderID)
        {
            List<OrderDetail> listOrderDetails;

            using (var connection = OpenConnection())
            {
                var sql = @"select * from OrderDetails where OrderID = @orderID";

                var parameters = new
                {
                    orderID
                };

                listOrderDetails = connection.Query<OrderDetail>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();

                connection.Close();
            }

            if (listOrderDetails == null)
                listOrderDetails = new List<OrderDetail>();

            return listOrderDetails;
        }

        public IList<OrderStatus> ListOrderStatus()
        {
            List<OrderStatus> listOrderStatuses;

            using (var connection = OpenConnection())
            {
                var sql = @"select * from OrderStatus";
                listOrderStatuses = connection.Query<OrderStatus>(sql: sql).ToList();

                connection.Close();
            }

            if (listOrderStatuses == null)
                listOrderStatuses = new List<OrderStatus>();

            return listOrderStatuses;
        }

        public bool Update(Order order)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Orders where OrderID = @OrderID)
                                select -1
                            else
                                begin
                                    update Orders 
                                    set CustomerID = @CustomerID,
                                        OrderTime = @OrderTime,
                                        DeliveryProvince = @DeliveryProvince,
                                        DeliveryAddress = @DeliveryAddress,
                                        EmployeeID = @EmployeeID,
                                        AcceptTime = @AcceptTime,
                                        ShipperID = @ShipperID,
                                        ShippedTime = @ShippedTime,
                                        FinishedTime = @FinishedTime,
                                        Status = @Status
                                    where OrderID = @OrderID
                                end";
                var parameters = new
                {
                    OrderID = order.OrderID,
                    CustomerID = order.CustomerID,
                    OrderTime = order.OrderTime,
                    DeliveryProvince = order.DeliveryProvince ?? "",
                    EmployeeID = order.EmployeeID,
                    DeliveryAddress = order.DeliveryAddress ?? "",
                    AcceptTime = order.AcceptTime,
                    ShipperID = order.ShipperID,
                    ShippedTime = order.ShippedTime,
                    FinishedTime = order.FinishedTime,
                    Status = order.Status,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
            return result;
        }

        public bool UpdateOrderDetail(OrderDetail orderDetail)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from OrderDetails where OrderID = @OrderID and @ProductID = ProductID)
                                select -1
                            else
                                begin
                                    update OrderDetails 
                                    set Quantity = @Quantity,
                                        SalePrice = @SalePrice
                                    where OrderID = @OrderID and @ProductID = ProductID
                                end";
                var parameters = new
                {
                    OrderID = orderDetail.OrderID,
                    ProductID = orderDetail.ProductID,
                    Quantity = orderDetail.Quantity,
                    SalePrice = orderDetail.SalePrice,
                    
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
            return result;
        }
    }
}
