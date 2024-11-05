using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class OrderProductDetail
    {
        [Key]
        public int OrderProductDetailId { get; set; }

        [ForeignKey("OrderProduct")]
        public int OrderProductId { get; set; }

        public virtual OrderProduct OrderProduct { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; } // Đổi thành kiểu decimal để đảm bảo tính toán giá chính xác

        public decimal TotalPrice
        {
            get
            {
                return Quantity * UnitPrice;
            }
        }
    }
}