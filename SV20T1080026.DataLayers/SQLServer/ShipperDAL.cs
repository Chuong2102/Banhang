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
    public class ShipperDAL : _BaseDAL, ICommonDAL<Shipper>
    {
        public ShipperDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Shipper data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Shippers where ShipperName = @ShipperName)
                                select -1
                            else
                                begin
                                    insert into Shippers(ShipperName,Phone)
                                    values(@ShipperName,@Phone);
                                    select @@identity;
                                end";
                var parameters = new
                {
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone ?? ""

                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;

            if (!searchValue.IsNullOrEmpty())
                searchValue = "%" + searchValue + "%";

            using (SqlConnection connection = OpenConnection())
            {
                var sql = @"
                    select count(*) from Shippers
                    where (@searchvalue = N'') or (ShipperName like @searchvalue)";

                var para = new
                {
                    searchValue = searchValue,
                };

                count = connection.ExecuteScalar<int>(sql: sql, param: para, commandType: CommandType.Text);

                connection.Close();
            }

            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = "delete from Shippers where ShipperID = @shipperId and not exists(select * from Orders where ShipperID = @shipperId)";
                var parameters = new { shipperId = id };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Shipper? Get(int id)
        {
            Shipper? data = null;
            using (var connection = OpenConnection())
            {
                var sql = "select * from Shippers where ShipperID = @shipperId";
                var parameters = new { shipperId = id };
                data = connection.QueryFirstOrDefault<Shipper>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Orders where ShipperID = @shipperId)
                                select 1
                            else 
                                select 0";
                var parameters = new { shipperId = id };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Shipper> listShippers;

            if (!searchValue.IsNullOrEmpty())
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"
                    with cte as
                    (
	                    select *, ROW_NUMBER() over (order by ShipperName) as RowNumber
	                    from Shippers
	                    where (@searchvalue = N'') or (ShipperName like @searchvalue)
                    )

                    select * from cte
                    where (@pageSize= 0)
	                    or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                    order by RowNumber;";

                var parameters = new
                {
                    page,
                    pageSize,
                    searchValue
                };

                listShippers = connection.Query<Shipper>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();

                connection.Close();
            }

            if (listShippers == null)
                listShippers = new List<Shipper>();

            return listShippers;
        }

        public bool Update(Shipper data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Shippers where ShipperID <> @shipperId and ShipperName = @shipperName)
                                begin
                                    update Shippers 
                                    set shipperName = @shipperName,
                                        Phone = @phone
                                    where ShipperId = @shipperId
                                end";
                var parameters = new
                {
                    shipperId = data.ShipperID,
                    shipperName = data.ShipperName ?? "",
                    phone = data.Phone
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
            return result;
        }
    }
}
