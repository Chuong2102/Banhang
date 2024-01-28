using SV20T1080026.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080026.DataLayers
{
    /// <summary>
    /// Dinh nghia cac phep xu ly lien quan den tai khoan
    /// </summary>
    public interface IUserAccountDAL
    {
        /// <summary>
        /// Xac thuc tai khoan cua nguoi dung (employye, customer)
        /// -> tra ve thong tin tai khoan
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserAccount? Authorize(string username, string password);
        /// <summary>
        /// Thay doi mat khau
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool ChangePassword(string username, string password);
    }
}
