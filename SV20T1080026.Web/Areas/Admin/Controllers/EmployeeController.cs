using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.RulesetToEditorconfig;
using SV20T1080026.BusinessLayers;
using SV20T1080026.DomainModels;
using SV20T1080026.Web.AppCodes;
using SV20T1080026.Web.Models;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace SV20T1080026.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    [Area("Admin")]
    public class EmployeeController : Controller
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        public EmployeeController(IWebHostEnvironment environment)
        {
            hostingEnvironment = environment;
        }

        private const int PAGE_SIZE = 9;
        const string EMPLOYEE_SEARCH = "Employee_Search";

        /// <summary>
        /// Trang chu cua nhan vien
        /// </summary>
        /// <returns></returns>
        //public IActionResult Index(int page = 1, string searchValue = "")
        //{
        //    int rowCount = 0;
        //    var data = CommonDataService.ListOfEmployees(out rowCount, page, PAGE_SIZE, searchValue ?? "");
        //    var model = new PaginationSearchEmployee()
        //    {
        //        Page = page,
        //        PageSize = PAGE_SIZE,
        //        SearchValue = searchValue ?? "",
        //        RowCount = rowCount,
        //        Data = data
        //    };

        //    string? ErrorMessage = Convert.ToString(TempData["ErrorMessage"]);
        //    ViewBag.ErrorMessage = ErrorMessage;

        //    return View(model);
        //}

        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH);

            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }

            return View(input);
        }

        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfEmployees(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");

            var model = new PaginationSearchEmployee()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            // Luu lai vao session dieu kien Tim kiem
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Thêm mới nhân viên";

            var model = new Employee()
            {
                EmployeeID = 0
            };

            return View(model);
        }

        public IActionResult ChangePassword(int id = 0)
        {
            return View();
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = CommonDataService.DeleteEmployee(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Không xóa thành công";
                    return RedirectToAction("Index");
                }

            }

            var model = CommonDataService.GetEmployee(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Chỉnh sửa thông tin nhân viên";

            var model = CommonDataService.GetEmployee(id);

            if (model == null)
                return RedirectToAction("Index");

            return View("Create", model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IActionResult Save(Employee data, string birthday, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Chỉnh sửa thông tin nhân viên";

            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Tên khách hàng không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email chỉ không được rỗng");
            if (string.IsNullOrWhiteSpace(data.BirthDate.ToString()))
                ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh không được rỗng");

            if (!ModelState.IsValid)
            {
                return View("Create", data);
            }

            //Xử lý ngày sinh
            DateTime? dBirthDate = AppCodes.Converter.StringToDateTime(birthday);
            if (dBirthDate == null)
                ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh không hợp lệ");
            else
                data.BirthDate = dBirthDate.Value;

            // Upload photo
            //
            if (uploadPhoto != null)
            {
                // Get path
                var filePath = Path.Combine(Path.Combine(hostingEnvironment.WebRootPath, "images"), "avatar");
                var fileName = Path.GetFileName(uploadPhoto.FileName);

                filePath = Path.Combine(filePath, fileName);
                data.Photo = "\\images\\avatar\\" + fileName;
                // Upload
                uploadPhoto.CopyTo(new FileStream(filePath, FileMode.Create));

            }

            if (!ModelState.IsValid)
                return View("Create", data);

            if (data.EmployeeID == 0)
            {
                var shipperID = CommonDataService.AddEmployee(data);

                if (shipperID > 0)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.ErrorMessage = "Không bổ sung thành công";
                return View("Create", data);
            }
            else
            {
                var success = CommonDataService.UpdateEmployee(data);

                if (success)
                    return RedirectToAction("Index");

                ViewBag.ErrorMessage = "Không cập nhật được dữ liệu";

                return View("Create", data);
            }


        }
    }
}
