using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    public class InvoiceService
    {
        private readonly AuthContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InvoiceService(AuthContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public Invoice getInvoice(int id)
        {

            var rs = _db.Invoices.Where(x => x.InvoiceId == id).FirstOrDefault();
            if (rs == null)
            {
                rs = new Invoice();
            }
            return rs;
        
            
        }
        public int SaveInvoice(List<CartItem> cartItems, Invoice invoice)
        {
            try
            {
                if (invoice.InvoiceId == 0) // Nếu hóa đơn mới chưa có Id, tạo mới
                {
                    // Tạo danh sách chi tiết hóa đơn từ giỏ hàng
                    List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>();
                    foreach (var item in cartItems)
                    {
                        invoiceDetails.Add(new InvoiceDetail()
                        {
                            Quantity = item.quantity, // Sử dụng Quantity từ CartItem
                            Total = item.quantity * item.product.Price, // Tính tổng cho mỗi chi tiết
                            Discount = 0, // Hoặc điều chỉnh nếu có chính sách giảm giá
                            ProductId = item.product.Id, // Lấy ProductId từ CartItem
                        });
                    }

                    // Gán danh sách chi tiết hóa đơn cho hóa đơn
                    invoice.InvoiceDetails = invoiceDetails;

                    // Thêm hóa đơn vào cơ sở dữ liệu
                    _db.Invoices.Add(invoice);

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _db.SaveChanges();

                    // Trả về ID của hóa đơn sau khi lưu
                    return invoice.InvoiceId; // Trả về ID của hóa đơn mới tạo
                }
                return 0; // Nếu hóa đơn đã tồn tại, trả về 0 (hoặc xử lý khác nếu cần)
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần thiết (có thể sử dụng logger)
                Console.WriteLine($"Error saving invoice: {ex.Message}");

                return 0; // Trả về 0 để chỉ ra lỗi
            }
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
        public async Task<Invoice> CreateInvoiceAsync(InvoiceDetailsViewModel model, List<CartItem> cartItems)
        {
            var invoice = new Invoice
            {
                UserId = model.UserId,
                DateTimeInvoice = DateTime.Now,

                InvoiceDetails = new List<InvoiceDetail>()
            };

            foreach (var item in cartItems)
            {
                invoice.InvoiceDetails.Add(new InvoiceDetail
                {
                    ProductId = item.product.Id,
                    Quantity = item.quantity,
                    Total = item.quantity * item.product.Price
                });
            }

            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();

            return invoice; // Trả về hóa đơn đã tạo
        }
    }
}

    //public int saveInvoice(List<CartItem> cartItems, Invoice invoice)
    //{
    //    // Thêm hóa đơn vào cơ sở dữ liệu
    //    _db.Invoices.Add(invoice);
    //    _db.SaveChanges(); // Lưu hóa đơn để lấy ID

    //    // Thêm các sản phẩm trong giỏ hàng vào chi tiết hóa đơn
    //    foreach (var item in cartItems)
    //    {
    //        var invoiceDetail = new InvoiceDetail
    //        {
    //            InvoiceId = invoice.InvoiceId,
    //        };
    //        _db.InvoiceDetails.Add(invoiceDetail);
    //    }

    //    // Lưu chi tiết hóa đơn vào cơ sở dữ liệu
    //    _db.SaveChanges();

    //    // Trả về ID của hóa đơn vừa tạo
    //    return invoice.InvoiceId;
    //}




