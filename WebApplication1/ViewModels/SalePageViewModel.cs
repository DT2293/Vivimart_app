using WebApplication1.Models;
using X.PagedList;

namespace WebApplication1.ViewModels
{
    public class SalePageViewModel
    {
        public int UserId { get; set; }
        public IPagedList<Product> Products { get; set; } 
        public Invoice Invoice { get; set; } = new Invoice();
        public List<CartItem> CartItems { get; set; }
    }
    //public class SalePageViewModel
    //{
    //    public InvoiceViewModel Invoice { get; set; }
    //    public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>(); // Initialize as empty
    //    public List<ProductViewModel> Products { get; set; }
    //}

    //public class CartItemViewModel
    //{
    //    public ProductViewModel Product { get; set; }
    //    public int Quantity { get; set; }
    //}
}
