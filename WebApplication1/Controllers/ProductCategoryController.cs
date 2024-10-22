using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services.SystemService;
using WebApplication1.ViewModels.ProductModels;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace WebApplication1.Controllers
{
    public class ProductCategoryController : Controller
    {
        ProductCategoryService _productCategoryService;

        public ProductCategoryController(ProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        public IActionResult Index(string keyWord)
        {
            ProductCategoryViewModel model = new ProductCategoryViewModel();
            List<ProductCategory> productCategories = new List<ProductCategory>();

            if (string.IsNullOrEmpty(keyWord))
            {
                productCategories = _productCategoryService.GetAll();
                
            }
            else
            {
                productCategories = _productCategoryService.GetByName(keyWord);
                model.keyWord = keyWord;
            }

            model.listCategory = productCategories;

            return View(model);
        }
    }
}
