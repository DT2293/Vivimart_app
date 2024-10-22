using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class InvoiceDetail
    {

        [Key]  // Khai báo Id là khóa chính
        public int InvoiceDetailId { get; set; } // mã sản phẩm

        public int Quantity { get; set; }
        public int Total { get; set; }
        public double Discount { get; set; }

        // Foreign key for Product
        public int ProductId { get; set; }  // Khóa ngoại tới Product
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        // Foreign key for Invoice
        public int InvoiceId { get; set; }  // Khóa ngoại tới Invoice
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

    }
}
