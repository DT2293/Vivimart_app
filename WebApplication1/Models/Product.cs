using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; } = null!;

        public DateTime CreateDate { get; set; }

        public DateTime ExpDate { get; set; }

        public int Price { get; set; }
        public int UserId { get; set; }
        // Thêm thuộc tính giảm giá
        public decimal? DiscountPercentage { get; set; } // Giảm giá phần trăm
        public DateTime? DiscountStartDate { get; set; } // Ngày bắt đầu giảm giá
        public DateTime? DiscountEndDate { get; set; }   // Ngày kết thúc giảm giá
        public int? SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Suppliers Supplier { get; set; } = null!;

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual ProductCategory ProductCategory { get; set; } 


        public ICollection<ProductImage> ProductImages { get; set; }

        public ICollection<InvoiceDetail> InvoiceDetails { get; set; }

        public ICollection<OrderProductDetail> OrderProductDetails { get; set; }
    }

}
