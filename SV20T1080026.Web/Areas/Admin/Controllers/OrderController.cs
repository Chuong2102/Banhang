using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080026.Web.AppCodes;
using SV20T1080026.Web.Models;
using SV20T1080026.Web;
using SV20T1080026.BusinessLayers;
using System.Drawing.Printing;

namespace SV20T1080026.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    [Area("Admin")]
    public class OrderController : Controller
    {
        private const string CART = "CART";
        const string ORDER_SEARCH = "Order_Search";
        const string ORDER = "Order";

        private const int PAGE_SIZE = 10;

        /// <summary>
        /// Hiển thị danh sách đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(ORDER_SEARCH);

            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CustomerID = 0,
                    EmployeeID = 0,
                };
            }

            return View(input);
        }

        public IActionResult Search(PaginationSearchInput input, int status)
        {
            int rowCount = 0;
            var data = OrderDataService.List(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "", status);

            var model = new PaginationSearchOrder()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data,
                CustomerID = input.CustomerID,
                EmployeeID = input.EmployeeID,
            };

            // Luu lai vao session dieu kien Tim kiem
            ApplicationContext.SetSessionData(ORDER_SEARCH, input);

            return View(model);
        }


        /// <summary>
        /// Giao diện trang tạo đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            var inputFromSession = ApplicationContext.GetSessionData<PaginationSearchInput>(ORDER_SEARCH);

            if (inputFromSession == null)
            {
                inputFromSession = new PaginationSearchInput()
                {
                    Page = 1,
                    SearchValue = "",
                    CustomerID = 0,
                    EmployeeID = 0,
                    PageSize= PAGE_SIZE,
                };
            }
            ApplicationContext.SetSessionData(ORDER_SEARCH, inputFromSession);

            return View(inputFromSession);
        }

        /// <summary>
        /// Tìm kiếm và hiển thị thông tin mặt hàng
        /// </summary>
        /// <param name="page"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>

        public IActionResult SearchProduct(PaginationSearchInput input)
        {
            int length = 5;

            //TODO: Search 
            var data = ProductDataService.ListProducts(input.SearchValue ?? "");

            var model = new PaginationSearchProduct()
            {
                Data = data.Skip(input.Page * length - length).Take(length).ToList(),
                Page = input.Page,
                SearchValue = input.SearchValue ?? "",
                PageSize = input.PageSize,
                CustomerID  = input.CustomerID,
                EmployeeID = input.EmployeeID,
            };

            // Luu lai vao session dieu kien Tim kiem
            ApplicationContext.SetSessionData(ORDER_SEARCH, input);

            return View(model);
        }
        /// <summary>
        /// Hiển thị giỏ hàng
        /// </summary>
        /// <returns></returns>

        public IActionResult ShowCart()
        {
            var model = GetCart();
            return View(model);
        }

        /// <summary>
        /// Lấy danh sách các mặt hàng trong giỏ
        /// </summary>
        /// <returns></returns>
        private List<CartItem> GetCart()
        {
            var cart = ApplicationContext.GetSessionData<List<CartItem>>(CART);
            if (cart == null)
            {
                cart = new List<CartItem>();
                ApplicationContext.SetSessionData(CART, cart);
            }
            return cart;
        }

        /// <summary>
        /// Giao diện trang chi tiết đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetByID(id);
            return View(order);
        }
        /// <summary>
        /// Giao diện cập nhật thông tin chi tiết đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <returns></returns>        
        [HttpGet]
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            // Get OrderDetail
            var orderDetail = OrderDataService.GetOrderDetail(id, productId);
            //

            return View(orderDetail);
        }
        /// <summary>
        /// Cập nhật chi tiết đơn hàng (trong giỏ hàng)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <param name="salePrice"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateDetail(int id = 0, int productId = 0, int quantity = 0, decimal salePrice = 0)
        {
            var orderDetail = OrderDataService.GetOrderDetail(id, productId);
            // Update
            orderDetail.SalePrice = salePrice;
            orderDetail.Quantity = quantity;

            OrderDataService.UpdateOrderDetail(orderDetail);

            return RedirectToAction("Details", new { id = id });
        }
        /// <summary>
        /// Xóa 1 mặt hàng khỏi giỏ hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productID"></param>
        /// <returns></returns>        
        public IActionResult DeleteDetail(int id = 0, int productID = 0)
        {
            OrderDataService.DeleteOrderDetail(id, productID);
            return RedirectToAction("Details", new { id = id });
        }
        /// <summary>
        /// Xóa đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            //TODO: Code chức năng để xóa đơn hàng (nếu được phép xóa)

            OrderDataService.DeleteOrderDetail(id);
            OrderDataService.Delete(id);

            return RedirectToAction("Index");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Accept(int id = 0)
        {
            //TODO: Duyệt chấp nhận đơn hàng

            var order = OrderDataService.GetByID(id);

            // Update AcceptTime and Status
            order.AcceptTime = DateTime.Now;
            order.Status = 2;

            OrderDataService.Update(order);

            return RedirectToAction("Details", new { id = id });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Shipping(int id = 0, int shipperID = 0)
        {
            if (Request.Method == "GET")
            {
                var order = OrderDataService.GetByID(id);

                return View(order);
            }
            else if(shipperID > 0)
            {
                //TODO: Chuyển đơn hàng cho người giao hàng
                var order = OrderDataService.GetByID(id);

                // Update Status and ShippedTime
                order.ShippedTime = DateTime.Now;
                order.Status = 3;
                order.ShipperID = shipperID;

                OrderDataService.Update(order);
            }

            return RedirectToAction("Details", new { id = id });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Finish(int id = 0)
        {
            //TODO: Ghi nhận hoàn tất đơn hàng
            var order = OrderDataService.GetByID(id);

            // Update Status and FinishedTime
            order.FinishedTime = DateTime.Now;
            order.Status = 4;

            OrderDataService.Update(order);

            return RedirectToAction($"Details", new { id = id });
        }
        /// <summary>
        /// Hủy bỏ đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Cancel(int id = 0)
        {
            //TODO: Hủy đơn hàng
            var order = OrderDataService.GetByID(id);

            // Update Status

            order.Status = -1;

            OrderDataService.Update(order);

            return RedirectToAction($"Details", new { id = id });
        }
        /// <summary>
        /// Từ chối đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Reject(int id = 0)
        {
            //TODO: Từ chối đơn hàng
            var order = OrderDataService.GetByID(id);

            // Update Status

            order.Status = -2;

            OrderDataService.Update(order);
            return RedirectToAction($"Details", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        public IActionResult SearchProducts()
        {
            return RedirectToAction("Create");
        }
        /// <summary>
        /// Bổ sung thêm hàng vào giỏ hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddToCart(CartItem data)
        {
            try
            {
                var cart = GetCart();
                int index = cart.FindIndex(m => m.ProductId == data.ProductId);
                if (index < 0)
                {
                    cart.Add(data);
                }
                else
                {
                    cart[index].Price = data.Price;
                    cart[index].Quantity += data.Quantity;
                }

                ApplicationContext.SetSessionData(CART, cart);
                return Json("");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        /// <summary>
        /// Xóa 1 mặt hàng khỏi giỏ hàng
        /// </summary>        
        /// <returns></returns>
        public ActionResult RemoveFromCart(string id)
        {
            var cart = GetCart();
            int index = cart.FindIndex(m => m.ProductId == id);
            if (index >= 0)
                cart.RemoveAt(index);
            ApplicationContext.SetSessionData(CART, cart);
            return Json("");
        }
        /// <summary>
        /// Xóa toàn bộ dữ liệu trong giỏ hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearCart()
        {
            var cart = GetCart();
            cart.Clear();
            ApplicationContext.SetSessionData(CART, cart);
            return Json("");
        }
        /// <summary>
        /// Khởi tạo đơn hàng và chuyển đến trang Details sau khi khởi tạo xong để tiếp tục quá trình xử lý đơn hàng
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        public ActionResult Init(int customerID, int employeeID)
        {
            // Get ds mat hang zo don hang
            var listProducts = GetCart();

            // Set error
            if(customerID == 0 || employeeID == 0 || listProducts == null)
            {
                TempData["ErrorInit"] = "Không thêm đơn hàng thành công, cần chọn đầy đủ thông tin";
                return RedirectToAction("Create");
            }

            // Tao don hang
            int orderId = OrderDataService.Add(new DomainModels.Order
            {
                CustomerID = customerID,
                EmployeeID = employeeID,
                AcceptTime = DateTime.Now,
                FinishedTime = DateTime.Now,
                ShippedTime = DateTime.Now,
                Status = 1,
                ShipperID = 1
            });


            foreach(var product in listProducts)
            {
                OrderDataService.AddOrderDetail(new DomainModels.OrderDetail
                {
                    OrderID = orderId,
                    ProductID = int.Parse(product.ProductId),
                    Quantity = product.Quantity,
                    SalePrice = product.Total
                });
            }

            //TODO: Khởi tạo đơn hàng và nhận mã đơn hàng được khởi tạo

            return RedirectToAction("Details", new { id = orderId });
        }
    }
}
