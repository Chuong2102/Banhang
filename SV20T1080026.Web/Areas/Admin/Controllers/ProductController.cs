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
    public class ProductController : Controller
    {
        private const string PRODUCT_SEARCH = "product_Search";
        private const string PRODUCT = "product";
        private const int PAGE_SIZE = 10;
        private readonly IWebHostEnvironment hostingEnvironment;

        public ProductController(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(PRODUCT_SEARCH);
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
        public IActionResult Search(PaginationSearchInput input, int categoryId, int supplierId)
        {
            long minPrice = 0;
            long maxPrice = 100000000;
            int rowCount = 0;

            var data = ProductDataService.ListProducts
                (
                    out rowCount,
                    input.Page,
                    input.PageSize,
                    input.SearchValue ?? "",
                    categoryId,
                    supplierId,
                    minPrice,
                    maxPrice
                );
            var model = new PaginationSearchProduct()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input); //Lưu lại điều kiện tìm kiếm

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Thêm mới mặt hàng";

            var model = new Product()
            {
                ProductId = 0
            };
            

            return View(model);
        }

        public IActionResult Save(Product data, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.ProductId == 0 ? "Bổ sung mặt hàng" : "Chỉnh sửa thông tin mặt hàng";

            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được rỗng");
            if (string.IsNullOrWhiteSpace(data.ProductDescription))
                ModelState.AddModelError(nameof(data.ProductDescription), "Mô tả không được rỗng");
            if (string.IsNullOrWhiteSpace(data.CategoryId.ToString()))
                ModelState.AddModelError(nameof(data.CategoryId), "Loại hàng không được rỗng");
            if (string.IsNullOrWhiteSpace(data.SupplierId.ToString()))
                ModelState.AddModelError(nameof(data.SupplierId), "Nhà cung cấp không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Price.ToString()))
                ModelState.AddModelError(nameof(data.Price), "Giá bán không được rỗng");

            if (!ModelState.IsValid)
            {
                return View("Create", data);
            }

            // Upload photo
            //
            if (uploadPhoto != null)
            {
                // Get path
                var filePath = Path.Combine(Path.Combine(hostingEnvironment.WebRootPath, "images"), "products");
                var fileName = Path.GetFileName(uploadPhoto.FileName);

                filePath = Path.Combine(filePath, fileName);
                data.Photo = "\\images\\products\\" + fileName;
                // Upload
                uploadPhoto.CopyTo(new FileStream(filePath, FileMode.Create));

            }

            if (!ModelState.IsValid)
                return View("Create", data);

            if (data.ProductId == 0)
            {
                var productId = ProductDataService.AddProduct(data);

                if (productId > 0)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.ErrorMessage = "Không bổ sung thành công";
                return View("Create", data);
            }
            else
            {
                var success = ProductDataService.UpdateProduct(data);

                if (success)
                    return RedirectToAction("Index");

                ViewBag.ErrorMessage = "Không cập nhật được dữ liệu";

                return View("Create", data);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int id = 0)
        {
            var model = ProductDataService.GetProduct(id);

            if (model == null)
                return RedirectToAction("Index");

            return View("Create", model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            // Xoa
            var isDelete = ProductDataService.DeleteProduct(id);

            if(!isDelete)
                ViewBag.ErrorMessage = "Không xóa được mặt hàng, có thể mặt hàng tồn tại trong đơn hàng nào đó!";

            // Get session
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }

            return View("Index", input);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="method"></param>
        /// <param name="photoId"></param>
        /// <returns></returns>
        public IActionResult Photo(int id = 0, string method = "add", int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh";
                    var model = new ProductPhoto()
                    {
                        PhotoId = 0,
                        ProductId = id
                    };
                    return View(model);
                case "edit":
                    var f = ProductDataService.GetPhoto(photoId);

                    if (f == null)
                        return RedirectToAction("Index");

                    return View(f);
                case "delete":
                    //TODO: Delete photo
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        public IActionResult AddPhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Mô tả không được rỗng");
            if (string.IsNullOrWhiteSpace(data.DisplayOrder.ToString()))
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị không được rỗng");

            // Upload photo
            //
            if (uploadPhoto != null)
            {
                // Get path
                var filePath = Path.Combine(Path.Combine(hostingEnvironment.WebRootPath, "images"), "products");
                var fileName = Path.GetFileName(uploadPhoto.FileName);

                filePath = Path.Combine(filePath, fileName);
                data.Photo = "\\images\\products\\" + fileName;
                // Upload
                if(!System.IO.File.Exists(filePath))
                    uploadPhoto.CopyTo(new FileStream(filePath, FileMode.Create));

            }


            if (!ModelState.IsValid)
                return View("RedirectToAction", new { id = data.ProductId });

            long photoId = 0;
            bool updated = false;

            if (data.PhotoId == 0)
            {
                photoId = ProductDataService.AddPhoto(data);
                return RedirectToAction("Edit", new { id = data.ProductId });
            }
            else
                updated = ProductDataService.UpdatePhoto(data);

            if (updated)
            {
                return RedirectToAction("Edit", new { id = data.ProductId });
            }

            ViewBag.ErrorMessage = "Không bổ sung ảnh thành công";
            return RedirectToAction("Edit", new { id = data.ProductId });


        }

        public IActionResult AddAttribute(ProductAttribute data)
        {
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được rỗng");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được rỗng");
            if (string.IsNullOrWhiteSpace(data.DisplayOrder.ToString()))
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị không được rỗng");

            

            if (!ModelState.IsValid)
                return View("RedirectToAction", new { id = data.ProductId });

            long atrID = 0;
            bool updated = false;

            if (data.AttributeId == 0)
            {
                atrID = ProductDataService.AddAttribute(data);
                return RedirectToAction("Edit", new { id = data.ProductId });
            }
            else
                updated = ProductDataService.UpdateAttribute(data);

            if (updated)
            {
                return RedirectToAction("Edit", new { id = data.ProductId });
            }

            ViewBag.ErrorMessage = "Không bổ sung thuộc tính thành công";
            return RedirectToAction("Edit", new { id = data.ProductId });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="method"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        public IActionResult Attribute(int id = 0, string method = "add", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính";
                    var model = new ProductAttribute()
                    {
                        AttributeId = 0,
                        ProductId = id
                    };
                    return View(model);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính";
                    var f = ProductDataService.GetAttribute(attributeId);

                    if (f == null)
                        return RedirectToAction("Index");

                    return View(f);
                case "delete":
                    //TODO: Delete Attribute
                    //TODO: Delete photo
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
    }
}
