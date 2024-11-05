using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.SystemService;
using WebApplication1.ViewModels;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AuthContext _db;
        private readonly SystemService _systemService;
        private readonly CustomPasswordHasher _customPasswordHasher;
        private readonly ProductServiceForS _productServiceForS;
        public HomeController(AuthContext db, ILogger<HomeController> logger, SystemService systemService, CustomPasswordHasher customPasswordHasher, ProductServiceForS productServiceForS)
        {
            _db = db;
            _logger = logger;
            _systemService = systemService;
            _customPasswordHasher = customPasswordHasher;
            _productServiceForS = productServiceForS;
        }
        public async Task<IActionResult> Index()
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
           
            return View("Index");
        }

            [HttpGet]
        public IActionResult CreateUser()
        {
            var viewModel = new CreateUserViewModel
            {
                User = new User(),
                Roles = _systemService.GetRoles()
            };

            return PartialView("_CreateUser", viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            var kt = _systemService.AddUserWithRoleAsync(model.User.Username, model.User.Password, model.User.Email, model.SelectedRole);
            if (kt == 0)
            {
                return Json(new { success = true });
            }
            else if (kt == 2)
            {
                model.Error.Add("Tài khoản đã tồn tại");
                return Json(new { success = false, errors = model.Error });
            }
            else
            {
                model.Error.Add("Mật khẩu chưa đủ 8 ký tự");
                return Json(new { success = false, errors = model.Error });
            }
        }

        //[HttpPost]
        //public IActionResult CreateUser(CreateUserViewModel model)
        //{
        //    var kt = _systemService.AddUserWithRoleAsync(model.User.Username, model.User.Password, model.User.Email, model.SelectedRole);
        //    if (kt == 0)
        //    {

        //        return RedirectToAction("Index");

        //    }
        //    else if (kt == 2)
        //    {
        //        model.Error.Add("tai khoan da ton tai");
        //        model.Roles = _systemService.GetRoles();
        //        return View(model);
        //    }
        //    else
        //    {
        //        model.Roles = _systemService.GetRoles();
        //        model.Error.Add("mat khau chua du 8 ky tu");
        //        return View(model);
        //    }
        //}

        [HttpGet]     
        public IActionResult ShowAllEmp(EmpoyeeViewModel data)
        {
            if (!string.IsNullOrEmpty(data.keyWord))
            {

                data.employeeViewModels = _systemService.GetByName(data.keyWord);
            }
            else
            {
                data.employeeViewModels = _systemService.GetAllEmp();
            }

            return PartialView(data);
        }


        [HttpGet]
        public IActionResult UpdateRole(int userId)
        {
            // Tìm người dùng theo UserId
            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);

            // Nếu không tìm thấy người dùng, trả về NotFound
            if (user == null)
            {
                return NotFound();
            }

            // Lấy danh sách vai trò hiện tại của người dùng
            List<string> currentRole = _db.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(x => x.RoleId.ToString())
                .ToList();

            // Khởi tạo model cho PartialView
            var model = new UpdateUserRoleViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                CurrentRole = currentRole,
                NewEmail = user.Email,
                NewRoleId = currentRole.FirstOrDefault(), // Chọn vai trò đầu tiên
                AvailableRoles = _db.Roles.ToList()
            };

            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(int userId, int newRoleId, string newEmail, string newPassword)
        {
            // Kiểm tra model state
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.SelectMany(x => x.Value.Errors).Select(e => e.ErrorMessage) });
            }

            // Cập nhật vai trò người dùng
            bool isUpdated = await _systemService.UpdateUserRoleAsync(userId, newRoleId, newEmail, newPassword);

            if (isUpdated)
            {
                return Json(new { success = true });
            }
            else
            {
                ModelState.AddModelError("", "Cập nhật vai trò không thành công.");
                return Json(new { success = false, errors = ModelState.SelectMany(x => x.Value.Errors).Select(e => e.ErrorMessage) });
            }
        }

        [HttpGet]
        public IActionResult UpdatePassword(int userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                ModelState.AddModelError("", "Người dùng không tồn tại.");
                return View(new UpdateAccountViewModel());
            }

            var model = new UpdateAccountViewModel
            {
                Username = user.Username
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdatePassword(UpdateAccountViewModel model)
        {
            string errorMessage;
            bool isUpdated = _systemService.UpdatePassword(model, out errorMessage);

            if (!isUpdated)
            {
                ModelState.AddModelError("", errorMessage);
                return View(model);
            }

            return RedirectToAction("ShowAllEmp");


        }

        [HttpGet]
        public IActionResult Applydiscount(int Id) // Sử dụng Id được truyền vào
        {
            // Tạo model với thông tin sản phẩm cần giảm giá
            var product = _productServiceForS.GetProductById(Id); // Sử dụng Id để lấy sản phẩm

            // Kiểm tra xem sản phẩm có tồn tại không
            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy, trả về NotFound
            }

            // Tạo model với thông tin sản phẩm cần giảm giá
            var model = new ProductViewModel
            {
                Id = product.Id, // Thêm Id vào model để sử dụng trong form
                ExpDate = product.ExpDate,
                Name = product.Name,
                Description = product.Description,
                DiscountPercentage = 0, // Khởi tạo giá trị mặc định cho tỷ lệ giảm giá
                DiscountStartDate = DateTime.Now, // Ngày bắt đầu mặc định là hôm nay
                DiscountEndDate = DateTime.Now.AddDays(7) // Ngày kết thúc mặc định là 7 ngày sau
            };

            return PartialView(model);
        }

        [HttpPost]
        public IActionResult Applydiscount(ProductViewModel model)
        {
            // Kiểm tra xem model có hợp lệ không
            if (ModelState.IsValid)
            {
                return PartialView(model); // Trả lại PartialView với thông tin đã nhập
            }

            // Lấy thông tin sản phẩm từ dịch vụ
            var product = _productServiceForS.GetProductById(model.Id);

            // Kiểm tra xem sản phẩm có tồn tại không
            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy, trả về NotFound
            }

            // Kiểm tra ngày bắt đầu và ngày kết thúc
            if (model.DiscountStartDate >= model.DiscountEndDate)
            {
                ModelState.AddModelError("", "Ngày bắt đầu phải trước ngày kết thúc.");
                return PartialView(model);
            }

            // Cập nhật thông tin giảm giá
            product.DiscountPercentage = model.DiscountPercentage; // Cập nhật tỷ lệ giảm giá
            product.DiscountStartDate = model.DiscountStartDate; // Cập nhật ngày bắt đầu
            product.DiscountEndDate = model.DiscountEndDate; // Cập nhật ngày kết thúc

            // Cập nhật sản phẩm trong cơ sở dữ liệu
            _productServiceForS.UpdateProduct(product); // Đảm bảo bạn đã có phương thức này trong service

            // Trả về một thông báo thành công hoặc cập nhật lại danh sách sản phẩm
            return View("ProductsExpiringSoon"); // Trả về một partial view thông báo thành công
        }
        // Controller action
        public IActionResult ProductsExpiringSoon()
        {
            try
            {
                int daysUntilExpiry = 30;
                var products = _productServiceForS.GetProductsExpiringSoon(daysUntilExpiry);

                if (products == null || !products.Any())
                {
                    ViewData["Message"] = "Không có sản phẩm nào sắp hết hạn.";
                    return View("NoProducts");
                }

                var productViewModels = products.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ExpDate = p.ExpDate,
                    Price = p.Price,
                    DiscountPercentage = p.DiscountPercentage
                }).ToList();

                return View(productViewModels);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = "Đã xảy ra lỗi khi tải danh sách sản phẩm.";
                return View("NoProducts");
            }
        }


        //public IActionResult ProductsExpiringSoon()

        //{
        //    try
        //    {
        //        int daysUntilExpiry = 30; // Cấu hình số ngày sắp hết hạn
        //        var products = _productServiceForS.GetProductsExpiringSoon(daysUntilExpiry);

        //        // Debug: Kiểm tra danh sách sản phẩm
        //        if (products == null || !products.Any())
        //        {
        //            // Trả về một view thông báo không có sản phẩm
        //            return View("NoProducts");
        //        }
        //       var productViewModels = products.Select(p => new ProductViewModel
        //    {
        //        Id = p.Id,
        //        Name = p.Name,
        //        Description = p.Description,
        //        ExpDate = p.ExpDate,
        //        Price = p.Price,
        //        DiscountPercentage = p.DiscountPercentage
        //    }).ToList();

        //    return View(productViewModels);
        //    }
        //    catch (Exception ex) 
        //    {  return View("NoProducts");
        //    }

        //}
        //public IActionResult ProductsExpiringSoon()
        //{
        //    var products = _productServiceForS.GetProductsExpiringSoon(); // Lấy danh sách sản phẩm
        //    if (products == null || !products.Any())
        //    {
        //        return View(new List<ProductViewModel>()); // Trả về danh sách rỗng nếu không có sản phẩm
        //    }

        //    var productViewModels = products.Select(p => new ProductViewModel
        //    {
        //        Id = p.Id,
        //        Name = p.Name,
        //        Description = p.Description,
        //        ExpDate = p.ExpDate,
        //        Price = p.Price,
        //        DiscountPercentage = p.DiscountPercentage
        //    }).ToList();

        //    return View(productViewModels);
        //}


        //public IActionResult ProductsExpiringSoon()
        //{
        //    int daysUntilExpiry = 30; // Cấu hình số ngày sắp hết hạn
        //    var products = _productServiceForS.GetProductsExpiringSoon(daysUntilExpiry);

        //    // Chuyển đổi từ Product sang ProductViewModel
        //    var productViewModels = products.Select(p => new ProductViewModel
        //    {
        //        SupplierID = p.SupplierId,
        //        ExpDate = p.ExpDate,
        //        Name = p.Name,
        //        Description = p.Description
        //    }).ToList();

        //    return View(productViewModels);
        //}



        public async Task<IActionResult> Delete(int id)
        {
           await _systemService.Delete(id);

            return RedirectToAction("ShowAllEmp");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

