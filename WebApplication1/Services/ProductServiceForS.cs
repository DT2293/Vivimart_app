using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using X.PagedList.Extensions;
using X.PagedList;
using Newtonsoft.Json;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    public class ProductServiceForS
    {
        private readonly AuthContext _db;
        public ProductServiceForS(AuthContext db)
        {
            _db = db;
        }
        public Product GetProductById(int Id)
        {
            return _db.Products.FirstOrDefault(p => p.Id == Id);
        }


        public IPagedList<Product> ListProduct(int? categoryId, int page = 1, int pageSize = 100)
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
            var data = query.Select(x => new Product()
            {
                
                Name = x.Name,
                Price = x.Price,
                ProductCategory = x.ProductCategory,
                ProductImages = x.ProductImages // Tải luôn ảnh sản phẩm
            }).ToList();

            return data;
        }
        public async Task<InvoiceDetailsViewModel> GetInvoiceDetailsAsync(int invoiceId, int userId)
        {
            
            var viewModel = new InvoiceDetailsViewModel
            {
                UserId = userId,
                InvoiceId = invoiceId,
                DateTimeInvoice = DateTime.Now 
            };
            viewModel.Invoice = await _db.Invoices.FindAsync(invoiceId);

            var invoiceDetails = await (from id in _db.InvoiceDetails
                                        join p in _db.Products on id.ProductId equals p.Id
                                        where id.InvoiceId == invoiceId
                                        select new CartItemViewModel
                                        {
                                            Name = p.Name,
                                            Price = p.Price,
                                            Quantity = id.Quantity
                                        }).ToListAsync();
            viewModel.Items = invoiceDetails;

            viewModel.CartItems = invoiceDetails.Select(item => new CartItem
            {
                product = new Product
                {
                    Name = item.Name,
                    Price = item.Price
                },
                quantity = item.Quantity
            }).ToList();

            return viewModel;
        }

        public List<Product> GetProductsExpiringSoon(int daysUntilExpiry)
        {
            return _db.Products
                .Where(p => EF.Functions.DateDiffDay(DateTime.Now, p.ExpDate) <= daysUntilExpiry
                            && p.ExpDate >= DateTime.Now)
                .OrderBy(p => p.ExpDate)
                .ToList();
        }

        public void UpdateProduct(Product product)
        {

            var existingProduct = _db.Products.Find(product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.ExpDate = product.ExpDate;
                existingProduct.Price = product.Price;
                existingProduct.UserId = product.UserId;
                existingProduct.SupplierId = product.SupplierId;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.DiscountPercentage = product.DiscountPercentage;
                existingProduct.DiscountStartDate = product.DiscountStartDate;
                existingProduct.DiscountEndDate = product.DiscountEndDate;
                _db.SaveChanges();
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

    }
}
