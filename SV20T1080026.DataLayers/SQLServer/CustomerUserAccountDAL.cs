using Dapper;
using SV20T1080026.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080026.DataLayers.SQLServer
{
    public class CustomerUserAccountDAL : _BaseDAL, IUserAccountDAL
    {
        public CustomerUserAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;

            using (var connection = OpenConnection())
            {
                var sql = @"select CustomerID as UserId, Email as UserName, CustomerName, Email from Customers where Email = @username and Password = @password";
                var parameters = new { username = username, password = password };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool ChangePassword(string username, string password)
        {
            int result;

            using (var connection = OpenConnection())
            {
                var sql = @"update Customers set Password = @password where Email = @username";
                var parameters = new { username = username, password = password };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            if (result > 0)
            {
                return true;
            }

            return false;
        }
    }
}
