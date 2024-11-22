using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.SystemService;
using WebApplication1.ViewModels;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace WebApplication1.Controllers
{

    public class ProductCategoryController : Controller
    {
        private readonly ProductCategoryService _productCategoryService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthContext _db;
        public ProductCategoryController(ProductCategoryService productCategoryService, UserManager<IdentityUser> userManager, AuthContext db)
        {
            _productCategoryService = productCategoryService;
            _userManager = userManager;
            _db = db;
        }
        public IActionResult Index(string keyWord, int userId)
        {

            ProductCategoryViewModel model = new ProductCategoryViewModel();

            List<ProductCategory> productCategories = new List<ProductCategory>();

            if (string.IsNullOrEmpty(keyWord))
            {
                productCategories = _productCategoryService.GetAll() ?? new List<ProductCategory>();
            }
            else
            {

                productCategories = _productCategoryService.GetByName(keyWord) ?? new List<ProductCategory>();
                model.keyWord = keyWord;  // Lưu từ khóa tìm kiếm vào ViewModel
            }

            model.listCategory = productCategories;


            return View(model);
        }
        [HttpGet]
        public IActionResult InsertCategory()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new ProductCategoryViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> InsertCategory(ProductCategoryViewModel model)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var username = HttpContext.Session.GetString("Username");

            if (!ModelState.IsValid)
            {
                // Gọi service để thêm danh mục
                bool isSuccess = await _productCategoryService.InsertCategoryAsync(model.CategoryName, username);

                if (isSuccess)
                {
                    return RedirectToAction("Index"); // Chuyển hướng sau khi thêm thành công
                }
                else
                {
                    return NotFound(); // Trường hợp không tìm thấy người dùng
                }
            }

            // Nếu dữ liệu không hợp lệ, quay lại form với thông báo lỗi
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            _productCategoryService.Delete(id);

            return RedirectToAction("Index");
        }

    }
}
