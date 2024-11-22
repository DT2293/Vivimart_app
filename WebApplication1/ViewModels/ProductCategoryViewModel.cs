using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class ProductCategoryViewModel
    {
        [Required]
        public string CategoryName { get; set; }
        public List<ProductCategory> listCategory { get; set; }
        public string keyWord { get; set; }
        public int UserId { get; set; }
        public ProductCategory selectedCategory { get; set; }
    }
}
