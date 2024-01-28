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
    public class SupplierDAL : _BaseDAL, ICommonDAL<Supplier>
    {
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Supplier data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Suppliers where Email = @Email)
	                            select -1
                            else
	                            begin
		                            insert into Suppliers(SupplierName, ContactName, Provice, Address, Phone, Email)
		                            values (@SupplierName, @ContactName, @Provice, @Address, @Phone, @Email)
		                            select @@IDENTITY
	                            end";
                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Provice = data.Provice ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? ""
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
                    select count(*) from Suppliers
                    where (@searchvalue = N'') or (SupplierName like @searchvalue)";

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
                var sql = "delete from Suppliers where SupplierID = @SupplierID and not exists(select * from Suppliers where SupplierID = @SupplierID)";
                var parameters = new { SupplierID = id };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Supplier? Get(int id)
        {
            Supplier? data = null;
            using (var connection = OpenConnection())
            {
                var sql = "select * from Suppliers where SupplierId = @supplierId";
                var parameters = new { supplierId = id };
                data = connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where SupplierID = @supplierId)
                                select 1
                            else 
                                select 0";
                var parameters = new { supplierId = id };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> listSuppliers;

            if (!searchValue.IsNullOrEmpty())
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"
                    with cte as
                    (
	                    select *, ROW_NUMBER() over (order by SupplierName) as RowNumber
	                    from Suppliers
	                    where (@searchvalue = N'') or (SupplierName like @searchvalue)
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

                listSuppliers = connection.Query<Supplier>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();

                connection.Close();
            }

            if (listSuppliers == null)
                listSuppliers = new List<Supplier>();

            return listSuppliers;
        }

        public bool Update(Supplier data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Suppliers where SupplierID <> @supplierId and Email = @email)
                                begin
                                    update Suppliers 
                                    set SupplierName = @supplierName,
                                        ContactName = @contactName,
                                        Provice = @provice,
                                        Address = @address,
                                        Phone = @phone,
                                        Email = @email
                                    where SupplierID = @supplierId
                                end";
                var parameters = new
                {
                    supplierId = data.SupplierID,
                    supplierName = data.SupplierName ?? "",
                    contactName = data.ContactName ?? "",
                    Provice = data.Provice ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? ""
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
            return result;
        }
    }
}
