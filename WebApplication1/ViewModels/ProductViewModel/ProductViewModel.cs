
using Infrastructure.Models;
using WebApplication1.Models;
using X.PagedList;

namespace WebApplication1.ViewModels.ProductViewModel
{
    public class ProductViewModel
    {
        //public string keyWord { get; set; }
        //public List<Product> products { get; set; }
        ////public Product selectedProduct { get; set; }
        ////public List<OptionModel> options { get; set; }

        //public int Id { get; set; }
        //public string Name { get; set; }
        //public string Description { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public DateTime ExpDate { get; set; }
        //public int CategoryId { get; set; }
        //public int UserId { get; set; }

        //public IFormFile FileUpload { get; set; }
        //public string[] DeleteImages { get; set; } // Ảnh đánh dấu xóa
        //public List<string> ExistingImages { get; set; } = new List<string>();
        //public int? SelectedCategoryId { get; set; }
        //public List<OptionModel> options { get; set; }
        //public IPagedList<Product> Products { get; set; }

        public IPagedList<Product> Products { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpDate { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public IFormFile FileUpload { get; set; }
        public string[] DeleteImages { get; set; }
        public List<string> ExistingImages { get; set; } = new List<string>();
        public int? SelectedCategoryId { get; set; }
        public List<OptionModel> options { get; set; }
    }
}
