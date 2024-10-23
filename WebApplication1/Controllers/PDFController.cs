//using DinkToPdf;
//using DinkToPdf.Contracts;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.ViewEngines;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Microsoft.EntityFrameworkCore;
//using WebApplication1.Models;
//using WebApplication1.ViewModels;

//public class PDFController : Controller
//{
//    private readonly IConverter _converter;
//    private readonly AuthContext _db;
//    public PDFController(IConverter converter, AuthContext db)
//    {
//        _converter = converter;
//        _db = db;
//    }

//    [HttpGet]
//    public IActionResult ExportInvoicePdf(int id)
//    {
//        var invoice = _db.Invoices.Include(i => i.InvoiceDetails)
//                 .ThenInclude(d => d.Product)
//                 .FirstOrDefault(i => i.InvoiceId == id);

//        if (invoice == null)
//        {
//            return NotFound();
//        }

//        // Ánh xạ từ Invoice sang InvoiceDetailsViewModel
//        var invoiceDetailsViewModel = new InvoiceDetailsViewModel
//        {
//            InvoiceId = invoice.InvoiceId,
//            UserId = invoice.UserId,
//            DateTimeInvoice = invoice.DateTimeInvoice,
//            Items = invoice.InvoiceDetails.Select(detail => new CartItemViewModel // Sử dụng CartItemViewModel
//            {
//                Name = detail.Product.Name,
//                Price = detail.Product.Price,
//                Quantity = detail.Quantity,
//            }).ToList(),
//        };

//        // Tính tổng số tiền
//        var Totalamount = invoice.InvoiceDetails.Sum(d => d.Quantity * d.Product.Price);
//        // Tạo nội dung HTML từ view hoặc từ chuỗi HTML trực tiếp
//        var htmlContent = RenderViewToString("InvoiceDetails", invoiceDetailsViewModel);

//        // Tạo PDF từ HTML
//        var pdf = new HtmlToPdfDocument()
//        {
//            GlobalSettings = {
//                PaperSize = PaperKind.A4,
//                Orientation = Orientation.Portrait,
//            },
//            Objects = {
//                new ObjectSettings() {
//                    HtmlContent = htmlContent
//                }
//            }
//        };

//        var pdfFile = _converter.Convert(pdf);
//        return File(pdfFile, "application/pdf", "invoice.pdf");
//    }

//    private string RenderViewToString(string viewName, object model)
//    {
//        ViewData.Model = model;
//        using (var sw = new StringWriter())
//        {
//            var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
//            var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw, new HtmlHelperOptions());
//            viewResult.View.RenderAsync(viewContext);
//            return sw.GetStringBuilder().ToString();
//        }
//    }
//}
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.ViewModels;

public class PDFController : Controller
{
    private readonly IConverter _converter;
    private readonly AuthContext _db;
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public PDFController(IConverter converter, AuthContext db, IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
    {
        _converter = converter;
        _db = db;
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    [HttpGet]
    public IActionResult ExportInvoicePdf(int id)
    {
        var invoice = _db.Invoices.Include(i => i.InvoiceDetails)
            .ThenInclude(d => d.Product)
            .FirstOrDefault(i => i.InvoiceId == id);

        if (invoice == null)
        {
            return NotFound();
        }

        var invoiceDetailsViewModel = new InvoiceDetailsViewModel
        {
            InvoiceId = invoice.InvoiceId,
            UserId = invoice.UserId,
            DateTimeInvoice = invoice.DateTimeInvoice,
            Items = invoice.InvoiceDetails.Select(detail => new CartItemViewModel
            {
                Name = detail.Product.Name,
                Price = detail.Product.Price,
                Quantity = detail.Quantity,
            }).ToList(),
        };

        var htmlContent = RenderViewToString("InvoiceDetails", invoiceDetailsViewModel);

        var pdf = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait,
            },
            Objects = {
                new ObjectSettings() {
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" },
                   //Stylesheets = { "https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" }
                }
            }
        };

        var pdfFile = _converter.Convert(pdf);
        return File(pdfFile, "application/pdf", "invoice.pdf");
    }

    private string RenderViewToString(string viewName, object model)
    {
        ViewData.Model = model;
        using (var sw = new StringWriter())
        {
            var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

            if (!viewResult.Success)
            {
                throw new InvalidOperationException($"Could not find view: {viewName}");
            }

            var viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                ViewData,
                TempData,
                sw,
                new HtmlHelperOptions()
            );

            viewResult.View.RenderAsync(viewContext).Wait();
            return sw.GetStringBuilder().ToString();
        }
    }
}
