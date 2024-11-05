using Infrastructure.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.SystemService;
using X.PagedList.Extensions;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly ProductCategoryService _productCategoryService;
        public readonly AuthContext _db;
        public readonly SupplierService _supplierService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ProductService productService, ProductCategoryService productCategoryService, AuthContext db, IWebHostEnvironment webHostEnvironment, SupplierService supplierService)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _supplierService = supplierService;
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
            var suppliers = _supplierService.GetAll(pageNumber: 1, pageSize: 7).Select(x => new OptionModel()
            {
                Value = x.SupplierID.ToString(),
                Text = x.SupplierName
            }).ToList();

            var categories = _productCategoryService.GetAll().Select(x => new OptionModel()
            {
                Value = x.CategoryId.ToString(),
                Text = x.CategoryName
            }).ToList();
                
            var model = new ProductViewModel
            {
                Suppliers = suppliers,
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
                Price = model.Price,
                CreateDate = model.CreatedDate,
                ExpDate = model.ExpDate,
                ProductCategory = _db.ProductCategories.Find(model.CategoryId),
                UserId = model.UserId,
                SupplierId = model.SupplierID
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
                //CategoryId = product.ProductCategory?.CategoryId? null,
              CategoryId = product.ProductCategory.CategoryId,
                UserId = product.UserId,
                SupplierID = product.Supplier?.SupplierID?? null,
                options = _productCategoryService.GetAll().Select(x => new OptionModel()
                {
                    Value = x.CategoryId.ToString(),
                    Text = x.CategoryName
                }).ToList(),
                Suppliers = _supplierService.GetAll(pageNumber: 1, pageSize: 7).Select(s => new OptionModel()
                {
                    Value = s.SupplierID.ToString(),
                    Text = s.SupplierName
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
                // Nếu dữ liệu không hợp lệ, trả lại danh sách các danh mục và nhà cung cấp
                model.options = _productCategoryService.GetAll().Select(x => new OptionModel()
                {
                    Value = x.CategoryId.ToString(),
                    Text = x.CategoryName
                }).ToList();
                model.Suppliers = _supplierService.GetAll(pageNumber: 1, pageSize: 7).Select(s => new OptionModel()
                {
                    Value = s.SupplierID.ToString(),
                    Text = s.SupplierName
                }).ToList();

                return View(model);
            }

            var product = await _db.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == model.Id);
            if (product == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin sản phẩm
            product.Name = model.Name;
            product.Description = model.Description;
            product.CreateDate = model.CreatedDate;
            product.ExpDate = model.ExpDate;
            product.ProductCategory = _db.ProductCategories.Find(model.CategoryId);
            product.UserId = model.UserId;
            product.SupplierId = model.SupplierID;

            // Xóa hình ảnh nếu có
            if (DeleteImages != null && DeleteImages.Any())
            {
                foreach (var fileName in DeleteImages)
                {
                    var image = product.ProductImages.FirstOrDefault(img => img.FileName == fileName);
                    if (image != null)
                    {
                        _db.ProductImages.Remove(image);
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "Image", fileName);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }
            }

            // Thêm hình ảnh mới nếu có
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

            // Lưu thay đổi
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        //[HttpGet]
        //public async Task<IActionResult> Update(int id)
        //{
        //    var product = await _db.Products
        //        .Include(p => p.ProductCategory)
        //        .Include(p => p.ProductImages)
        //        .FirstOrDefaultAsync(p => p.Id == id);

        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    var model = new ProductViewModel
        //    {
        //        Id = product.Id,
        //        Name = product.Name,
        //        Description = product.Description,
        //        CreatedDate = product.CreateDate,
        //        ExpDate = product.ExpDate,
        //        CategoryId = product.ProductCategory.CategoryId,
        //        UserId = product.UserId,
        //        options = _productCategoryService.GetAll().Select(x => new OptionModel()
        //        {
        //            Value = x.CategoryId.ToString(),
        //            Text = x.CategoryName
        //        }).ToList(),
        //        Suppliers = _productCategoryService.GetAll().Select(x => new OptionModel()
        //        {
        //            Value = x.CategoryId.ToString(),
        //            Text = x.CategoryName
        //        }).ToList(),
        //        ExistingImages = product.ProductImages.Select(img => img.FileName).ToList()
        //    };

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Update(ProductViewModel model, List<string> DeleteImages)
        //{
        //    if (!ModelState.IsValid)
        //    {

        //        model.options = _productCategoryService.GetAll().Select(x => new OptionModel()
        //        {
        //            Value = x.CategoryId.ToString(),
        //            Text = x.CategoryName
        //        }).ToList();

        //    }

        //    var product = await _db.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == model.Id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }


        //    product.Name = model.Name;
        //    product.Description = model.Description;
        //    product.CreateDate = model.CreatedDate;
        //    product.ExpDate = model.ExpDate;
        //    product.ProductCategory = _db.ProductCategories.Find(model.CategoryId);
        //    product.UserId = model.UserId;
        //    product.SupplierId = model.SupplierID;
        //    await _db.SaveChangesAsync();

        //    if (model.FileUpload != null)
        //    {
        //        var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
        //                       + Path.GetExtension(model.FileUpload.FileName);

        //        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "Image", fileName);

        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await model.FileUpload.CopyToAsync(fileStream);
        //        }
        //        _db.ProductImages.Add(new ProductImage
        //        {
        //            ProductId = product.Id,
        //            FileName = fileName
        //        });
        //    }

        //    await _db.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));


        //}



    }
}




