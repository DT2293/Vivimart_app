using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{

    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; } // Khóa chính cho hóa đơn

        // Ngày giờ lập hóa đơn, mặc định là thời điểm hiện tại
        public DateTime DateTimeInvoice { get; set; } = DateTime.Now;

        // Khóa ngoại tới User
        // Tham chiếu đến người dùng
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        // Danh sách chi tiết hóa đơn
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    }
    
    
}
