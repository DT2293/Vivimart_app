using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;


namespace WebApplication1.Models
{
    public class Suppliers
    {
        [Key]
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Tên nhà cung cấp là bắt buộc.")]
        public string SupplierName { get; set; }

        [Required(ErrorMessage = "Tên liên hệ là bắt buộc.")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Vui lòng nhập Fax")]
        public string Fax { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    }

}
