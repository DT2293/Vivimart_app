using WebApplication1.Models;
using X.PagedList;

namespace WebApplication1.ViewModels
{
    public class SupplierViewModel
    {
        public int SupplierID { get; set;   }
        public Suppliers suppliers { get; set; } = new Suppliers();
        public string SupplierName { get; set; }
        public string ContactName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string keyWord { get; set; }
        public IPagedList<Suppliers> listSupplier { get; set; }
    }
}
