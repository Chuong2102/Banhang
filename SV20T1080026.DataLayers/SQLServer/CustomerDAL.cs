﻿using SV20T1080026.DomainModels;

using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.IdentityModel.Tokens;

namespace SV20T1080026.DataLayers.SQLServer
{
    public class CustomerDAL : _BaseDAL, ICommonDAL<Customer>
    {
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Customers(CustomerName,ContactName,Province,Address,Phone,Email,IsLocked)
                                    values(@CustomerName,@ContactName,@Province,@Address,@Phone,@Email,@IsLocked);
                                    select @@identity;
                                end";
                var parameters = new
                {
                    customerName = data.CustomerName ?? "",
                    contactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Count(string searchValue = "")
        {
            int count = 0;

            if (!searchValue.IsNullOrEmpty())
                searchValue = "%" + searchValue + "%";

            using (SqlConnection connection = OpenConnection())
            {
                var sql = @"
                    select count(*) from Customers
                    where (@searchvalue = N'') or (CustomerName like @searchvalue)";

                var para = new
                {
                    searchValue = searchValue,
                };

                count = connection.ExecuteScalar<int>(sql: sql, param: para, commandType: CommandType.Text);

                connection.Close();
            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = "delete from Customers where CustomerId = @customerId and not exists(select * from Orders where CustomerId = @customerId)";
                var parameters = new { customerId = id };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Customer? Get(int id)
        {
            Customer? data = null;
            using (var connection = OpenConnection())
            {
                var sql = "select * from Customers where CustomerId = @customerId";
                var parameters = new { customerId = id };
                data = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public string GetNameByID(int id)
        {
            string? data = "";
            using (var connection = OpenConnection())
            {
                var sql = "select CustomerName from Customers where CustomerId = @customerId";
                var parameters = new { customerId = id };

                data = connection.QueryFirstOrDefault<string>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Orders where CustomerId = @customerId)
                                select 1
                            else 
                                select 0";
                var parameters = new { customerId = id };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IList<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> listCustomers;

            if(!searchValue.IsNullOrEmpty())
                searchValue = "%" + searchValue + "%";

            using(var connection = OpenConnection())
            {
                var sql = @"
                    with cte as
                    (
	                    select *, ROW_NUMBER() over (order by CustomerName) as RowNumber
	                    from Customers
	                    where (@searchvalue = N'') or (CustomerName like @searchvalue)
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

                listCustomers = connection.Query<Customer>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();

                connection.Close();
            }

            if (listCustomers == null)
                listCustomers = new List<Customer>();

            return listCustomers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Update(Customer data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Customers where CustomerId <> @customerId and Email = @email)
                                begin
                                    update Customers 
                                    set CustomerName = @customerName,
                                        ContactName = @contactName,
                                        Province = @province,
                                        Address = @address,
                                        Phone = @phone,
                                        Email = @email,
                                        IsLocked = @isLocked
                                    where CustomerId = @customerId
                                end";
                var parameters = new
                {
                    customerId = data.CustomerID,
                    customerName = data.CustomerName ?? "",
                    contactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
            return result;
        }
    }
}
