using System;
using System.Collections.Generic;
using WebApplication1.Models;
using X.PagedList;

namespace WebApplication1.ViewModels
{
    public class InvoiceDetailsViewModel
{
    public int UserId { get; set; }
    public IPagedList<Product> Products { get; set; }
    public Invoice Invoice { get; set; } = new Invoice();
    public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    public int InvoiceId { get; set; }
    public DateTime DateTimeInvoice { get; set; } = DateTime.Now;

    // Thêm thuộc tính cho danh sách sản phẩm
    public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
    public List<InvoiceDetail> Details { get; set; }
        // Phương thức tính tổng giá trị hóa đơn
        public int GetTotal()
    {
       

        int total = 0;

        // Tính tổng giá trị hóa đơn
        foreach (var item in CartItems)
        {
            total += item.product.Price * item.quantity;
        }
        return total;
    }
}


    public class CartItemViewModel
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
