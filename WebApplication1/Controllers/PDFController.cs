using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Helper;
using WebApplication1.Models;
using System.IO;
using System.Threading.Tasks;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class PDFController : Controller
    {
        private readonly IConverter _converter;
        private readonly ProductServiceForS _productServiceForS;

        // Inject IConverter dependency via constructor
        public PDFController(IConverter converter,ProductServiceForS productServiceForS)
        {
            _converter = converter;

        }

        [HttpGet]
        public async Task<IActionResult> GeneratePdf()
        {
            // Đường dẫn lưu file PDF sau khi tạo
            string filename = "Invoice.pdf";
            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "Exports", filename);

            // Cài đặt chung cho tài liệu PDF
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom = 10 },
                DocumentTitle = "Invoice",
                Out = outputPath // Lưu file PDF vào thư mục Exports
            };

            // Cài đặt riêng cho nội dung PDF
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
   //             HtmlContent = CustomerInvoice.ToHtmlFile(InvoiceDetail.getdata()), // Nội dung HTML chuyển đổi từ dữ liệu
                WebSettings = { DefaultEncoding = "utf-8" }, // Cài đặt encoding
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]" },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "This is a footer." }
            };

            // Tạo tài liệu HTML to PDF
            var pdf = new HtmlToPdfDocument
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            // Chuyển đổi nội dung sang PDF
            _converter.Convert(pdf);

            // Kiểm tra xem file PDF đã tồn tại và trả về để tải xuống
            if (System.IO.File.Exists(outputPath))
            {
                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(outputPath);
                return File(fileBytes, "application/pdf", filename);
            }

            // Trường hợp file không được tạo ra
            return NotFound();
        }
    }
}
