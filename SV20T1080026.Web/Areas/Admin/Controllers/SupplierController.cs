using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080026.BusinessLayers;
using SV20T1080026.DomainModels;
using SV20T1080026.Web.Models;
using System.Data;

namespace SV20T1080026.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    [Area("Admin")] //Contribute
    public class SupplierController : Controller
    {
        private const int PAGE_SIZE = 10;

        /// <summary>
        /// Hiển thị trang nhà cung cấp
        /// </summary>
        /// <returns></returns>
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfSuppliers(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            var model = new PaginationSearchSupplier()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            string? ErrorMessage = Convert.ToString(TempData["ErrorMessage"]);
            ViewBag.ErrorMessage = ErrorMessage;

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";

            var model = new Supplier()
            {
                SupplierID = 0
            };

            return View(model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhà cung cấp";

            var model = CommonDataService.GetSupplier(id);

            if (model == null)
                return RedirectToAction("Index");

            return View("Create", model);
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = CommonDataService.DeleteSupplier(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Không xóa thành công";
                    return RedirectToAction("Index");
                }

            }

            var model = CommonDataService.GetSupplier(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        public IActionResult Save(Supplier data)
        {
            ViewBag.Title = data.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Chỉnh sửa thông tin nhà cung cấp";

            if (string.IsNullOrWhiteSpace(data.SupplierName))
                ModelState.AddModelError(nameof(data.SupplierName), "Tên nhà cung cấp không được rỗng");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Provice))
                ModelState.AddModelError(nameof(data.Provice), "Vui lòng chọn tỉnh thành");

            if (!ModelState.IsValid)
            {
                return View("Create", data);
            }

            if (data.SupplierID == 0)
            {
                var customerID = CommonDataService.AddSupplier(data);

                if (customerID > 0)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.ErrorMessage = "Không bổ sung thành công";
                return View("Create", data);
            }
            else
            {
                var success = CommonDataService.UpdateSupplier(data);

                if (success)
                    return RedirectToAction("Index");

                ViewBag.ErrorMessage = "Không cập nhật được dữ liệu";

                return View("Create", data);
            }


        }
    }
}
