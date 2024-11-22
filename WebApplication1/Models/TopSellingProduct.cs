using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models;
public class TopSellingProduct
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int TotalQuantitySold { get; set; }
    public int Price { get; set; }  // Giữ kiểu decimal nếu Price là số thập phân
    public int CategoryId { get; set; }
    public int Revenue { get; set; }  // Đảm bảo Revenue là int
}