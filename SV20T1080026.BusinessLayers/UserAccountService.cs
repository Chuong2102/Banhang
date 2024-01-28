using SV20T1080026.DataLayers;
using SV20T1080026.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080026.BusinessLayers
{
    public static class UserAccountService
    {
        static readonly IUserAccountDAL employeeUserAccountDB;
        static readonly IUserAccountDAL customerUserAccountDB;

        static UserAccountService()
        {
            string connectionString = "server=LAPTOP-HP5KFASE\\SQLEXPRESS;user id=sa;password=1;database=LiteCommerceDB;TrustServerCertificate=true";

            employeeUserAccountDB = new DataLayers.SQLServer.EmployeeUserAccountDAL(connectionString);
            customerUserAccountDB = new DataLayers.SQLServer.CustomerUserAccountDAL(connectionString);
        }

        public static UserAccount? Authorize(string username, string password, TypeOfAccount typeOfAccount)
        {
            switch (typeOfAccount)
            {
                case TypeOfAccount.Employee:
                    return employeeUserAccountDB.Authorize(username, password);
                //customer
                case TypeOfAccount.Customer:
                    return customerUserAccountDB.Authorize(username, password);
                default: return null;
            }
        }

        public static bool ChangePassword(string username, string password, TypeOfAccount typeOfAccount)
        {
            switch (typeOfAccount)
            {
                case TypeOfAccount.Employee:
                    return employeeUserAccountDB.ChangePassword(username, password);
                // customer
                case TypeOfAccount.Customer:
                    return customerUserAccountDB.ChangePassword(username, password);
                default: return false;
            }
        }
    }
    
    public enum TypeOfAccount
    {
        Employee,
        Customer
    }
}
