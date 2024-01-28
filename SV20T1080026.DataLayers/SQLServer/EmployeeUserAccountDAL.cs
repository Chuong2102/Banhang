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
    /// <summary>
    /// Cai dat cac phep xu ly lien quan den tai khoan cua Employee
    /// </summary>
    public class EmployeeUserAccountDAL : _BaseDAL, IUserAccountDAL
    {
        public EmployeeUserAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount Authorize(string username, string password)
        {
            UserAccount? data = null;

            using (var connection = OpenConnection())
            {
                var sql = @"select EmployeeID as UserId, Email as UserName, FullName, Email, Photo from Employees where Email = @username and Password = @password";
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
                var sql = @"update Employees set Password = @password where Email = @username";
                var parameters = new { username = username, password = password };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            
            if(result > 0)
            {
                return true;
            }

            return false;
        }
    }
}
