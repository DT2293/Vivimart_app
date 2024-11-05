using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace WebApplication1.Models
{
    public class OrderProduct
    {
        [Key]
        public int OrderProductId { get; set; }

        public DateTime OrderTime { get; set; }  // Đổi kiểu dữ liệu thành DateTime

        public EStatus Status { get; set; }

        [ForeignKey("SupplierID")]
        public int SupplierID { get; set; }

        public virtual Suppliers Supplier { get; set; }

        [ForeignKey("UserID")]
        public int UserID { get; set; }

        public virtual User User { get; set; }
    }
    public enum EStatus
    {
        Ordered, Transit, Received

    }
}