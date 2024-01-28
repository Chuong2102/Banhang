using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080026.BusinessLayers;
using SV20T1080026.DomainModels;
using SV20T1080026.Web.AppCodes;
using SV20T1080026.Web.Models;
using System.Data;
using System.Drawing.Printing;

namespace SV20T1080026.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    [Area("Admin")]
    public class ShipperController : Controller
    {
        private const int PAGE_SIZE = 10;
        const string SHIPPER_SEARCH = "Shipper_Search";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        //public IActionResult Index(int page = 1, string searchValue = "")
        //{
        //    int rowCount = 0;
        //    var data = CommonDataService.ListOfShippers(out rowCount, page, PAGE_SIZE, searchValue ?? "");
        //    var model = new PaginationSearchShipper()
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
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH);

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
            var data = CommonDataService.ListOfShippers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");

            var model = new PaginationSearchShipper()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            // Luu lai vao session dieu kien Tim kiem
            ApplicationContext.SetSessionData(SHIPPER_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Thêm mới đơn vị giao hàng";

            var model = new Shipper()
            {
                ShipperID = 0
            };

            return View(model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Chỉnh sửa thông tin đơn vị giao hàng";

            var model = CommonDataService.GetShipper(id);

            if (model == null)
                return RedirectToAction("Index");

            return View("Create", model);
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = CommonDataService.DeleteShipper(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Không xóa thành công";
                    return RedirectToAction("Index");
                }

            }

            var model = CommonDataService.GetShipper(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        public IActionResult Save(Shipper data)
        {
            ViewBag.Title = data.ShipperID == 0 ? "Bổ sung đơn vị giao hàng" : "Chỉnh sửa thông tin đơn vị giao hàng";

            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Tên đơn vị giao hàng không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được rỗng");

            if (!ModelState.IsValid)
            {
                return View("Create", data);
            }

            if (data.ShipperID == 0)
            {
                var shipperID = CommonDataService.AddShipper(data);

                if (shipperID > 0)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.ErrorMessage = "Không bổ sung thành công";
                return View("Create", data);
            }
            else
            {
                var success = CommonDataService.UpdateShipper(data);

                if (success)
                    return RedirectToAction("Index");

                ViewBag.ErrorMessage = "Không cập nhật được dữ liệu";

                return View("Create", data);
            }


        }
    }
}
