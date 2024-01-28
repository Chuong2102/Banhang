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
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 10;
        const string CATEGORY_SEARCH = "Category_Search";

        /// <summary>
        /// Hiển thị trang loại hàng
        /// </summary>
        /// <returns></returns>
        //public IActionResult Index(int page = 1, string searchValue = "")
        //{
        //    int rowCount = 0;
        //    var data = CommonDataService.ListOfCategories(out rowCount, page, PAGE_SIZE, searchValue ?? "");
        //    var model = new PaginationSearchCategory()
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
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH);

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
            var data = CommonDataService.ListOfCategories(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");

            var model = new PaginationSearchCategory()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            // Luu lai vao session dieu kien Tim kiem
            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Tạo mới loại hàng";

            var model = new Category
            {
                CategoryID = 0
            };

            return View(model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin loại hàng";

            var model = CommonDataService.GetCategory(id);

            if (model == null)
                return RedirectToAction("Index");

            return View("Create", model);
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = CommonDataService.DeleteCategory(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Không xóa thành công";
                    return RedirectToAction("Index");
                }

            }

            var model = CommonDataService.GetCategory(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        public IActionResult Save(Category data)
        {
            ViewBag.Title = data.CategoryID == 0 ? "Bổ sung loại hàng" : "Chỉnh sửa thông tin loại hàng";

            if (string.IsNullOrWhiteSpace(data.CategoryName))
                ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Mô tả không được rỗng");

            if (!ModelState.IsValid)
            {
                return View("Create", data);
            }

            if (data.CategoryID == 0)
            {
                var customerID = CommonDataService.AddCategory(data);

                if (customerID > 0)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.ErrorMessage = "Không bổ sung thành công";
                return View("Create", data);
            }
            else
            {
                var success = CommonDataService.UpdateCategory(data);

                if (success)
                    return RedirectToAction("Index");

                ViewBag.ErrorMessage = "Không cập nhật được dữ liệu";

                return View("Create", data);
            }


        }


    }
}
