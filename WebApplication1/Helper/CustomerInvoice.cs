using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Helper
{
    public class CustomerInvoice
    {
        public static string ToHtmlFile(List<InvoiceDetail> data, string invoiceId, DateTime invoiceDate, decimal totalAmount)
        {

            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "HTMLTemplate", "index.html");

            // Đọc file HTML template
            string tempHtml = File.ReadAllText(templatePath);

            // Xây dựng phần HTML chứa các sản phẩm
            StringBuilder sb = new StringBuilder();
            foreach (var item in data)
            {
                sb.Append($@"
                <tr>
                    <td>{item.Product}</td>
                    <td>{item.Product.Price}</td>
                    <td>{item.Quantity}</td>
                    <td>{item.Quantity * item.Product.Price}</td>
                </tr>");
            }

            // Thay thế các placeholder trong file HTML bằng dữ liệu thực
            string htmlContent = tempHtml
                .Replace("{data}", sb.ToString()) // Thay thế dữ liệu sản phẩm
                .Replace("{data}", invoiceId) // Thay thế mã hóa đơn
                .Replace("{data}", invoiceDate.ToString("dd/MM/yyyy")) // Thay thế ngày hóa đơn
                .Replace("{data}", totalAmount.ToString("C")); // Thay thế tổng tiền

            return htmlContent;
 
        }
    }
}

