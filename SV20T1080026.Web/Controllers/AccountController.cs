using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080026.BusinessLayers;

namespace SV20T1080026.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string username = "", string password = "")
        {
            ViewBag.UserMame = username;
            ViewBag.Password = password;

            // Kiem tra thong tin dang nhap dung hay sai
            //TODO: Kiem tra UserName, Pass tu CSDL

            var userAccount = UserAccountService.Authorize(username, password, TypeOfAccount.Employee);
            //var userAccount = UserAccountService.Authorize(username, password, TypeOfAccount.Customer);

            if (userAccount != null)
            {
                // Dang nhap thanh cong
                // Tao doi tuong luu cac thong tin cua phien dang nhap
                WebUserData userData = new WebUserData()
                {
                    UserId = userAccount.UserId,
                    UserName = userAccount.UserName,
                    DisplayName = userAccount.FullName,
                    Email = userAccount.Email,
                    Photo = userAccount.Photo,
                    ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    SessionId = HttpContext.Session.Id,
                    AdditionalData = "",
                    Roles = new List<string> { WebUserRoles.Administrator}
                };

                await HttpContext.SignInAsync(userData.CreatePrincipal());

                return RedirectToAction("Index", "Dashboard", new {area = "Admin"});
            }
            else
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();

        }

        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string comfirmNewPassword)
        {
            bool result;
            string username = User?.GetUserData().UserName ?? "";

            //
            var userAccount = UserAccountService.Authorize(username, oldPassword, TypeOfAccount.Employee);
            //var userAccount = UserAccountService.Authorize(username, oldPassword, TypeOfAccount.Customer);

            if (userAccount != null)
            {
                if(newPassword != comfirmNewPassword)
                {

                    ModelState.AddModelError("Error0", "Mật khẩu mới không trùng nhau");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("Error1", "Mật khẩu cũ chưa đúng");
                return View();
            }


            result = UserAccountService.ChangePassword(username, newPassword, TypeOfAccount.Employee);
            //result = UserAccountService.ChangePassword(username, newPassword, TypeOfAccount.Customer);
            if (result) 
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            return View();
        }
    }
}
