using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using X.PagedList.Extensions;
using X.PagedList;

namespace WebApplication1.Services
{
    public class ProductService
    {
        private readonly AuthContext _db;
        public ProductService(AuthContext db)
        {
            _db = db;
        }
        //public List<Product> ListProduct()
        //{
        //    var data = _db.Products.Select(x => new Product()
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        Description = x.Description,
        //        CreateDate= x.CreateDate,
        //        ExpDate= x.ExpDate,
        //        UserId= x.UserId,
        //        ProductCategory = x.ProductCategory,

        //    }).ToList();
        //    return data;
        //}

        public Product GetProductById(int id)
        {
            return _db.Products
                      .Include(p => p.ProductImages)
                      .FirstOrDefault(p => p.Id == id);
        }
        public List<Product> GetByName(string keyWord)
        {
            List<Product> obj = _db.Products.Where(x => x.Name.Contains(keyWord)).ToList();

            return obj;
        }



        //public List<Product> ListProduct()
        //{
        //    var data = _db.Products
        //                  .Include(p => p.ProductImages) // Tải luôn các ảnh liên quan
        //                  .Select(x => new Product()
        //                  {
        //                      Id = x.Id,
        //                      Name = x.Name,
        //                      Description = x.Description,
        //                      CreateDate = x.CreateDate,
        //                      ExpDate = x.ExpDate,
        //                      UserId = x.UserId,
        //                      ProductCategory = x.ProductCategory,
        //                      ProductImages = x.ProductImages
        //                  })
        //                  .ToList();

        //    return data;
        //}


        public IPagedList<Product> ListProduct(int? categoryId, int page = 1, int pageSize = 10)
        {
            var query = _db.Products.AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.ProductCategory.CategoryId == categoryId.Value);
            }

            return query
                .Include(p => p.ProductImages) // Tải luôn các ảnh liên quan
                .ToPagedList(page, pageSize); // Phân trang
        }


        public List<Product> ListProduct(int? categoryId)
        {
            // Tạo query để lấy danh sách sản phẩm, bao gồm cả ProductImages
            var query = _db.Products.Include(p => p.ProductImages).AsQueryable();

            // Nếu categoryId có giá trị và lớn hơn 0, lọc theo danh mục
            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(p => p.ProductCategory.CategoryId == categoryId);
            }

            // Trả về danh sách các sản phẩm (sau khi đã lọc nếu có)
            var data = query.Select(x => new Product()
            {
                
                Name = x.Name,
                Description = x.Description,
                CreateDate = x.CreateDate,
                ExpDate = x.ExpDate,
                UserId = x.UserId,
                ProductCategory = x.ProductCategory,
                ProductImages = x.ProductImages // Tải luôn ảnh sản phẩm
            }).ToList();

            return data;
        }



        public void CreateProduct(Product product)
        {
            _db.Products.Add(product);
            _db.SaveChanges(); 
        }

        public void UpdateProduct(Product product)
        {
            _db.Products.Update(product);
            _db.SaveChanges(); 
        }

        public void DeleteProduct(int productId)
        {
            var product = _db.Products.Find(productId);
            if (product != null)
            {
                _db.Products.Remove(product);
                _db.SaveChanges(); 
            }
        }


    }
}


