using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; } // Khóa chính cho sản phẩm

        public string Name { get; set; } // Tên sản phẩm

        public string Description { get; set; } = null!; // Mô tả sản phẩm, yêu cầu không được null

        public DateTime CreateDate { get; set; } // Ngày tạo sản phẩm

        public DateTime ExpDate { get; set; } // Ngày hết hạn của sản phẩm

        public int Price { get; set; }
        public int UserId { get; set; } // Khóa ngoại tham chiếu đến User (người tạo sản phẩm)

        public virtual ProductCategory ProductCategory { get; set; } = null!; // Tham chiếu đến danh mục sản phẩm

        public ICollection<ProductImage> ProductImages { get; set; } // Danh sách hình ảnh của sản phẩm

        public ICollection<InvoiceDetail> InvoiceDetails { get; set; } // Danh sách chi tiết hóa đơn liên quan đến sản phẩm
    }
}
