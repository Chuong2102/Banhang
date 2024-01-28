using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080026.BusinessLayers;
using SV20T1080026.DomainModels;
using SV20T1080026.Web.AppCodes;
using SV20T1080026.Web.Models;

namespace SV20T1080026.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private const int PAGE_SIZE = 10;
        const string CUSTOMER_SEARCH = "Customer_Search";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public IActionResult Index()
        //{
        //    int rowCount = 0;
        //    var data = CommonDataService.ListOfCustomers(out rowCount, page, PAGE_SIZE, searchValue ?? "");
        //    var model = new PaginationSearchCustomer()
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
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH);

            if(input == null)
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
            var data = CommonDataService.ListOfCustomers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");

            var model = new PaginationSearchCustomer()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            // Luu lai vao session dieu kien Tim kiem
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Thêm mới khách hàng";

            var model = new Customer()
            {
                CustomerID = 0
            };

            var listProvinces = CommonDataService.ListOfProvinces();
            ViewBag.listOfProvinces = listProvinces;

            return View(model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Chỉnh sửa thông tin khách hàng";

            var model = CommonDataService.GetCustomer(id);

            if (model == null)
                return RedirectToAction("Index");

            var listProvinces = CommonDataService.ListOfProvinces();
            ViewBag.listOfProvinces = listProvinces;

            return View("Create", model);
        }

        public IActionResult Delete(int id = 0)
        {
            if(Request.Method == "POST")
            {
                bool success = CommonDataService.DeleteCustomer(id);
                if(!success)
                {
                    TempData["ErrorMessage"] = "Không xóa thành công";
                    return RedirectToAction("Index");
                }    

            }

            var model = CommonDataService.GetCustomer(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        public IActionResult ChangePassword(int id = 0)
        {
            return View();
        }

        public IActionResult Save(Customer data)
        {
            ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng" : "Chỉnh sửa thông tin khách hàng";

            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được rỗng");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành");

            if(!ModelState.IsValid)
            {
                return View("Create", data);
            }

            var listProvinces = CommonDataService.ListOfProvinces();
            ViewBag.listOfProvinces = listProvinces;

            if (data.CustomerID == 0)
            {
                var customerID = CommonDataService.AddCustomer(data);

                if (customerID > 0)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.ErrorMessage = "Không bổ sung thành công";
                return View("Create", data);
            }
            else { 
                var success = CommonDataService.UpdateCustomer(data);

                if(success)
                    return RedirectToAction("Index");

                ViewBag.ErrorMessage = "Không cập nhật được dữ liệu";

                return View("Create", data);
            }


        }

    }
}
