using WebApplication1.Models;

namespace WebApplication1.ViewModels.ProductModels
{
    public class ProductCategoryViewModel
    {
        public List<ProductCategory> listCategory { get; set; }
        public string keyWord { get; set; }
        public ProductCategory selectedCategory { get; set; }
    }
}
