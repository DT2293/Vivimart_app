using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ProductCategory
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int UserId { get; set; } 
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
