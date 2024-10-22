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
        public Product GetProductById(int id)
        {
            return _db.Products
                      .Include(p => p.ProductImages)
                      .FirstOrDefault(p => p.Id == id);
        }

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
            // Tạo đối tượng view model
            var viewModel = new InvoiceDetailsViewModel
            {
                UserId = userId,
                InvoiceId = invoiceId,
                DateTimeInvoice = DateTime.Now // Hoặc lấy ngày từ Invoice nếu có
            };

            // Lấy thông tin hóa đơn
            viewModel.Invoice = await _db.Invoices.FindAsync(invoiceId);

            // Lấy chi tiết hóa đơn
            var invoiceDetails = await (from id in _db.InvoiceDetails
                                        join p in _db.Products on id.ProductId equals p.Id
                                        where id.InvoiceId == invoiceId
                                        select new CartItemViewModel
                                        {
                                            Name = p.Name,
                                            Price = p.Price,
                                            Quantity = id.Quantity
                                        }).ToListAsync();

            // Gán danh sách sản phẩm vào view model
            viewModel.Items = invoiceDetails;

            // Lấy các sản phẩm trong giỏ hàng nếu cần
            // (Có thể tùy chỉnh theo logic của bạn)
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

    }
}
