using Infrastructure.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.SystemService;
using WebApplication1.ViewModels.ProductViewModel;
using X.PagedList.Extensions;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly ProductCategoryService _productCategoryService;
        public readonly AuthContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ProductService productService, ProductCategoryService productCategoryService, AuthContext db, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int? categoryId, int page = 1)
        {
            
            var categories = _productCategoryService.GetAll().Select(x => new OptionModel
            {
                Value = x.CategoryId.ToString(),
                Text = x.CategoryName
            }).ToList();

            var products = _productService.ListProduct(categoryId);

            // Phân trang danh sách sản phẩm
            int pageSize = 5; // Số sản phẩm trên mỗi trang
            var pagedProducts = products.ToPagedList(page, pageSize);

            // Tạo ProductViewModel
            var data = new ProductViewModel
            {
                Products = pagedProducts,
                options = categories,
                SelectedCategoryId = categoryId
            };

            return View(data);
        }

        //upload file 
        public class UploadOnfile
        {
            [Required(ErrorMessage = "Chon file de upload")]
            [DataType(DataType.Upload)]
            [FileExtensions(Extensions = "png,jpg,jpeg,gif")]
            [Display(Name = "chon file updload")]
            public IFormFile FileUpload { get; set; }
        }
        [HttpGet]
        public IActionResult UploadPhoto(int id)
        {
            var product = _db.Products.Where(e => e.Id == id).Include(p => p.ProductImages)
                                                .FirstOrDefault();
            if (product == null)
            {
                return NotFound("K co sp");
            }
            ViewData["product"] = product;

            return View(new UploadOnfile());
        }


        [HttpPost, ActionName("UploadPhoto")]
        public async Task<IActionResult> UploadPhotoAsync(int id, [Bind("FileUpload")] UploadOnfile f)
        {
            var product = _db.Products.Where(e => e.Id == id).Include(p => p.ProductImages)
                                                 .FirstOrDefault();
            if (product == null)
            {
                return NotFound("K co sp");
            }
            ViewData["product"] = product;

            if (f != null)
            {
                var file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                    + Path.GetExtension(f.FileUpload.FileName);

                var file = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "Image", file1);

                using (var filestream = new FileStream(file, FileMode.Create))
                {
                    await f.FileUpload.CopyToAsync(filestream);
                }
                _db.Add(new ProductImage()
                {
                    ProductId = product.Id,
                    FileName = file1,
                });
                await _db.SaveChangesAsync();
            }

            return View(new UploadOnfile());
        }

        [HttpPost]
        public IActionResult ListPhotos(int id)
        {
            var product = _db.Products.Where(e => e.Id == id).Include(p => p.ProductImages).FirstOrDefault();

            if (product == null)
            {
                return Json(
                    new
                    {
                        success = 0,
                        message = "not found"

                    }
                    );
            }
            var listphotos = product.ProductImages.Select(photo => new
            {
                id = photo.Id,
                path = "/assets/Image/"+photo.FileName
            });
            return Json(
               new
               {
                   success = 1,
                   photos = listphotos,
               }
                );
        }

        [HttpPost]
        public IActionResult DeletePhoto(int id)
        {
            var photo = _db.ProductImages.Where(p => p.Id == id).FirstOrDefault();
            if(photo != null)
            {
                _db.Remove(photo);
                _db.SaveChanges();
                // var filename = "/assets/Image/" + photo.FileName;
                string fileName = photo.FileName;
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/Image");
                string fullPath = Path.Combine(folderPath, fileName);

                System.IO.File.Delete(fullPath);

            }
            return Ok();
        }
        //    thêm
        [HttpGet]
        public IActionResult Create()
        {
            var categories = _productCategoryService.GetAll().Select(x => new OptionModel()
            {
                Value = x.CategoryId.ToString(),
                Text = x.CategoryName
            }).ToList();

            var model = new ProductViewModel
            {
                options = categories
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model)
        {

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                CreateDate = model.CreatedDate,
                ExpDate = model.ExpDate,
                ProductCategory = _db.ProductCategories.Find(model.CategoryId),
                UserId = model.UserId
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            if (model.FileUpload != null)
            {
                var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                               + Path.GetExtension(model.FileUpload.FileName);

                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "Image", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.FileUpload.CopyToAsync(fileStream);
                }

                _db.ProductImages.Add(new ProductImage
                {
                    ProductId = product.Id,
                    FileName = fileName
                });
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _db.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CreatedDate = product.CreateDate,
                ExpDate = product.ExpDate,
                CategoryId = product.ProductCategory.CategoryId,
                UserId = product.UserId,
                options = _productCategoryService.GetAll().Select(x => new OptionModel()
                {
                    Value = x.CategoryId.ToString(),
                    Text = x.CategoryName
                }).ToList(),

                ExistingImages = product.ProductImages.Select(img => img.FileName).ToList()
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Update(ProductViewModel model, List<string> DeleteImages)
        {
            if (!ModelState.IsValid)
            {

                model.options = _productCategoryService.GetAll().Select(x => new OptionModel()
                {
                    Value = x.CategoryId.ToString(),
                    Text = x.CategoryName
                }).ToList();
                return View(model);
            }

            var product = await _db.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == model.Id);
            if (product == null)
            {
                return NotFound();
            }


            product.Name = model.Name;
            product.Description = model.Description;
            product.CreateDate = model.CreatedDate;
            product.ExpDate = model.ExpDate;
            product.ProductCategory = _db.ProductCategories.Find(model.CategoryId);
            product.UserId = model.UserId;
            await _db.SaveChangesAsync();

            if (model.FileUpload != null)
            {
                var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                               + Path.GetExtension(model.FileUpload.FileName);

                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "Image", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.FileUpload.CopyToAsync(fileStream);
                }
                _db.ProductImages.Add(new ProductImage
                {
                    ProductId = product.Id,
                    FileName = fileName
                });
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

          
        }



    }
}




