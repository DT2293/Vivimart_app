
using Infrastructure.Models;
using WebApplication1.Models;
using X.PagedList;
namespace WebApplication1.ViewModels
{
    public class ProductViewModel
    {
        public IPagedList<Product> Products { get; set; }
        public int Id { get; set; }
        public int? SupplierID { get; set; }
        public string? SupplierName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpDate { get; set; }
        public int CategoryId { get; set; }
        public int Price { get; set; }
        public int UserId { get; set; }
        public IFormFile FileUpload { get; set; }
        public string[] DeleteImages { get; set; }
        public List<string> ExistingImages { get; set; } = new List<string>();
        public int? SelectedCategoryId { get; set; }
        public List<OptionModel> Suppliers { get; set; }
        public List<OptionModel> options { get; set; }

        public decimal? DiscountPercentage { get; set; } // Giảm giá phần trăm
        public DateTime? DiscountStartDate { get; set; } // Ngày bắt đầu giảm giá
        public DateTime? DiscountEndDate { get; set; }   // Ngày kết thúc giảm giá

 
    }
}
