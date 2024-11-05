using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.ViewModels.Employee;
using WebApplication1.Services;
using Newtonsoft.Json;
using WebApplication1.ViewModels;
using X.PagedList.Extensions;
using WebApplication1.Services.SystemService;
using System.Security.Claims;
using System.Numerics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using WebApplication1.Helper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApplication1.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AuthContext _db;
        private readonly ProductServiceForS _productServiceFs;
        private readonly SystemService _systemService;
        private readonly InvoiceService _invoiceService;
        private readonly IVnPayService _vnPayService;
        public EmployeeController(AuthContext db, ProductServiceForS productServiceForS, SystemService systemService, 
            InvoiceService invoiceService, IVnPayService vnPayService)
        {
            _db = db;
            _productServiceFs = productServiceForS;
            _systemService = systemService;
            _invoiceService = invoiceService;
            _vnPayService = vnPayService;  
            
        }
      
        public async Task<IActionResult> Employee()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var username = HttpContext.Session.GetString("Username");
            var userInfo = await _db.Users
                .Where(u => u.Username == username)
                .Select(u => new
                {
                    u.Username,
                    u.Email,
                    RoleName = _db.UserRoles
                        .Where(ur => ur.UserId == u.UserId)
                        .Join(_db.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => r.RoleName)
                        .FirstOrDefault(),
                    AccessType = _db.RoleMenus
                        .Where(rm => rm.RoleId == _db.UserRoles
                            .Where(ur => ur.UserId == u.UserId)
                            .Select(ur => ur.RoleId)
                            .FirstOrDefault())
                        .Select(rm => rm.AccessType)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (userInfo == null)
            {
                return NotFound();
            }

            ViewBag.Username = userInfo.Username;
            ViewBag.Email = userInfo.Email;
            ViewBag.RoleName = userInfo.RoleName;
            ViewBag.AccessType = userInfo.AccessType;
            return View("Employee");
        }



        public const string CARTKEY = "cart";

        // Lấy cart từ Session (danh sách CartItem)
        List<CartItem> GetCartItems()
        {

            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
            }
            return new List<CartItem>();
        }

        // Xóa cart khỏi session
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào session
        void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }
        public IActionResult SalePage(int? categoryId, int? invoiceId = null, int page = 1)
        {


            // Lấy danh sách sản phẩm
            var products = _productServiceFs.ListProduct(categoryId, page);

            // Lấy hóa đơn nếu invoiceId có giá trị
            //   var invoice = invoiceId.HasValue ? _productServiceFs.GetInvoiceById(invoiceId.Value) : null;

            // Tạo ViewModel
            var viewModel = new SalePageViewModel
            {
                Products = products.ToPagedList(page, 100),
                //   Invoice = invoice
            };

            // Trả về view với ViewModel
            return View(viewModel);
        }

        public IActionResult AddToCartPartial()
        {
            var model = GetCartItems();
            return PartialView(model);
        }
        public IActionResult UpdateCart(int productId, int quantity)
        {
            try
            {
                // Kiểm tra số lượng có hợp lệ không
                if (quantity <= 0)
                {
                    return BadRequest("Số lượng phải lớn hơn 0."); // Trả về lỗi nếu số lượng không hợp lệ
                }

                // Lấy danh sách sản phẩm từ Session
                var cartItems = GetCartItems();

                // Tìm sản phẩm trong giỏ hàng
                var cartItem = cartItems.FirstOrDefault(item => item.product.Id == productId);
                if (cartItem != null)
                {
                    // Cập nhật số lượng sản phẩm
                    cartItem.quantity = quantity;

                    // Lưu lại giỏ hàng vào Session
                    var jsonCart = JsonConvert.SerializeObject(cartItems);
                    HttpContext.Session.SetString(CARTKEY, jsonCart);
                }
                else
                {
                    return NotFound("Sản phẩm không tồn tại trong giỏ hàng."); // Trả về lỗi nếu sản phẩm không tìm thấy
                }

                return Json(new { success = true, product = cartItem.product }); // Trả về sản phẩm đã cập nhật
            }
            catch (Exception ex)
            {
                // Trả về lỗi
                return Json(new { success = false, errors = new[] { ex.Message } });
            }
        }




        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            // Lấy danh sách các sản phẩm trong giỏ hàng
            var cart = GetCartItems();

            // Tìm sản phẩm trong giỏ hàng
            var cartItem = cart.Find(p => p.product.Id == id);

            if (cartItem == null)
            {
                return NotFound("Sản phẩm không tồn tại trong giỏ hàng");
            }

            // Xóa sản phẩm khỏi giỏ hàng
            cart.Remove(cartItem);

            // Lưu lại giỏ hàng vào Session
            SaveCartSession(cart);

            // Trả về danh sách giỏ hàng hiện tại (có thể trả về PartialCart nếu cần)
            return Ok(cart); // Bạn có thể điều chỉnh tùy thuộc vào yêu cầu của ứng dụng
        }




        //add cart
        /// Thêm sản phẩm vào cart
        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            var product = _db.Products
         .Where(p => p.Id == id)
         .FirstOrDefault();

            if (product == null)
            {
                return NotFound("Không có sản phẩm");
            }

            // Xử lý đưa vào Cart ...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.Id == id);

            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity++;
            }
            else
            {
                // Thêm mới
                cart.Add(new CartItem() { quantity = 1, product = product });
            }

            // Lưu cart vào Session
            SaveCartSession(cart);
            var t = GetCartItems();

            // Trả về PartialCart thay vì Cart
            return Ok();
        }
        [Route("/cart", Name = "cart")]
        public IActionResult Cart()
        {
            return PartialView("AddToCart", GetCartItems());
        }


        [HttpGet]
        public IActionResult CreateInvoice()
        {
            // Check if user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get username from session
            var username = HttpContext.Session.GetString("Username");

            // Retrieve UserId based on username
            var userInfo = _db.Users
                .Where(u => u.Username == username)
                .Select(u => new { u.UserId })
                .FirstOrDefault();

            if (userInfo == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            // Get cart items
            var cartItems = GetCartItems();

            // Check if cart is empty
            if (cartItems == null || !cartItems.Any())
            {
                TempData["EmptyCartMessage"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi tạo hóa đơn.";
                return RedirectToAction("SalePage"); // Redirect to the cart page or wherever is suitable
            }
           
            // Prepare invoice details
            var invoiceDetails = new InvoiceDetailsViewModel
            {
                UserId = userInfo.UserId,
                DateTimeInvoice = DateTime.Now,
                CartItems = cartItems
            };

            return View(invoiceDetails);
        }



        [HttpPost]
        public async Task<IActionResult> CreateInvoice(InvoiceDetailsViewModel model, VnPaymentRequestModel model2, string payment = "Thanh toán vnpay")
        {
            var cart = GetCartItems();
            if (cart == null || !cart.Any())
            {
                ModelState.AddModelError("", "Giỏ hàng trống.");
                return View(model);
            }
            //var totalAmount = model.GetTotal();
            if (!ModelState.IsValid)
            {
                // Xử lý thanh toán qua VNPay
                if (payment == "Thanh toán vnpay")
                {
                    var totalAmount = cart.Sum(item => item.product.Price * item.quantity);


                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = totalAmount, // Tổng tiền của hóa đơn
                        CreatedDate = DateTime.Now,
                        OrderId = new Random().Next(1000, 100000) // Tạo ID hóa đơn ngẫu nhiên
                    };

                    // Chuyển hướng đến trang thanh toán của VNPay
                    return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
                }


                var invoice = await _invoiceService.CreateInvoiceAsync(model, cart);
                if (invoice == null)
                {
                    ModelState.AddModelError("", "Lỗi khi tạo hóa đơn.");
                    return View(model);
                }
                _db.Database.BeginTransaction();
                try
                {
                    // Thêm hóa đơn vào cơ sở dữ liệu
                    _db.Add(invoice);
                    _db.SaveChanges(); // Lưu hóa đơn trước để lấy `InvoiceId`
                    // Tạo danh sách chi tiết hóa đơn từ giỏ hàng
                    var invoiceDetails = new List<InvoiceDetail>();
                    foreach (var item in cart)
                    {
                        invoiceDetails.Add(new InvoiceDetail
                        {
                            InvoiceId = invoice.InvoiceId, // Liên kết với hóa đơn vừa tạo
                            Quantity = item.quantity, // Số lượng sản phẩm
                                                      //   price = item.product.Price, // Giá sản phẩm
                            ProductId = item.product.Id // Liên kết với sản phẩm
                        });
                    }

                    // Thêm chi tiết hóa đơn vào cơ sở dữ liệu
                    _db.AddRange(invoiceDetails);
                    _db.SaveChanges(); // Lưu tất cả các chi tiết hóa đơn

                    // Cam kết giao dịch
                    _db.Database.CommitTransaction();

                    // Xóa giỏ hàng sau khi hoàn thành thanh toán
                    ClearCart();

                    // Chuyển hướng tới trang thành công
                    return View("PaymentSuccess");
                }
                catch (Exception ex)
                {
                    // Rollback giao dịch nếu có lỗi xảy ra
                    _db.Database.RollbackTransaction();

                    // Log lỗi hoặc thêm thông báo lỗi cho người dùng
                    ModelState.AddModelError("", "Đã có lỗi xảy ra trong quá trình xử lý: " + ex.Message);
                }
            }

            // Nếu có lỗi hoặc model không hợp lệ, quay lại trang giỏ hàng
            return View("SalePage");
        }
        [HttpGet] 
		[Route("CreateInvoice/PaymentCallBack")]
		public async Task<IActionResult> PaymentCallBack(string vnp_Amount, string vnp_BankCode, string vnp_BankTranNo,
									  string vnp_CardType, string vnp_OrderInfo, string vnp_PayDate,
									  string vnp_ResponseCode, string vnp_TmnCode, string vnp_TransactionNo,
									  string vnp_TransactionStatus, string vnp_TxnRef, string vnp_SecureHash)
		{
			// Xử lý phản hồi từ VNPay
			var response = _vnPayService.PaymentExecute(Request.Query);

			// Kiểm tra mã phản hồi
			if (response == null || response.VnPayResponseCode != "00")
			{
				TempData["Message"] = $"Lỗi thanh toán VN Pay: {response?.VnPayResponseCode}";
				return RedirectToAction("PaymentFail");
			}

			// Lấy giỏ hàng từ session
			var cart = GetCartItems();
			if (cart == null || !cart.Any())
			{
				TempData["Message"] = "Giỏ hàng trống. Không thể lưu hóa đơn.";
				return RedirectToAction("SalePage");
			}

			// Lấy thông tin người dùng từ session
			var username = HttpContext.Session.GetString("Username");
			var userInfo = _db.Users.FirstOrDefault(u => u.Username == username);
			if (userInfo == null)
			{
				TempData["Message"] = "Người dùng không tồn tại.";
				return RedirectToAction("SalePage");
			}

			// Tạo đối tượng hóa đơn
			var invoice = new Invoice
			{
				UserId = userInfo.UserId,
				DateTimeInvoice = DateTime.Now,
			};

			_db.Database.BeginTransaction();
			try
			{
				// Lưu hóa đơn vào database
				_db.Invoices.Add(invoice);
				await _db.SaveChangesAsync(); // Lưu hóa đơn để lấy InvoiceId

				// Lưu chi tiết hóa đơn
				var invoiceDetails = new List<InvoiceDetail>();
				foreach (var item in cart)
				{
					invoiceDetails.Add(new InvoiceDetail
					{
						InvoiceId = invoice.InvoiceId,
						ProductId = item.product.Id,
						Total = item.product.Price * item.quantity,
						Quantity = item.quantity
					});
				}

				// Thêm danh sách chi tiết hóa đơn vào cơ sở dữ liệu
				_db.InvoiceDetails.AddRange(invoiceDetails);
				await _db.SaveChangesAsync(); // Lưu các chi tiết hóa đơn

				// Commit giao dịch
				_db.Database.CommitTransaction();

				// Xóa giỏ hàng sau khi thanh toán thành công
				ClearCart();

				// Thông báo thành công
				TempData["Message"] = "Thanh toán VNPay thành công";
				return RedirectToAction("InvoiceDetails", new { id = invoice.InvoiceId });

            }
			catch (Exception ex)
			{
				// Rollback giao dịch nếu có lỗi xảy ra
				_db.Database.RollbackTransaction();

				// Log lỗi hoặc thêm thông báo lỗi
				TempData["Message"] = $"Lỗi khi lưu hóa đơn: {ex.Message}";
				return RedirectToAction("PaymentFail");
			}
		}

		public IActionResult PaymentFail()
		{
			return View();
		}
		public IActionResult PaymentSuccess()
        {
            return View();
        }
		public async Task<IActionResult> InvoiceDetails(int id)
        {
            var invoice =_db.Invoices.Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefault(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            // Ánh xạ từ Invoice sang InvoiceDetailsViewModel
            var invoiceDetailsViewModel = new InvoiceDetailsViewModel
            {
                InvoiceId = invoice.InvoiceId,
                UserId = invoice.UserId,
                DateTimeInvoice = invoice.DateTimeInvoice,
                Items = invoice.InvoiceDetails.Select(detail => new CartItemViewModel // Sử dụng CartItemViewModel
                {
                    Name = detail.Product.Name,
                    Price = detail.Product.Price,
                    Quantity = detail.Quantity,
                }).ToList(),
            };

            // Tính tổng số tiền
           var Totalamount =  invoice.InvoiceDetails.Sum(d => d.Quantity * d.Product.Price);

            return View(invoiceDetailsViewModel); // Trả về InvoiceDetailsViewModel
        }

    }
}
