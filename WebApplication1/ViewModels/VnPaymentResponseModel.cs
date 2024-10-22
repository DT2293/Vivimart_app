using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
    }
    public class VnPaymentRequestModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public int OrderId { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public int GetTotalVnpay()
        {


            int total = 0;

            // Tính tổng giá trị hóa đơn
            foreach (var item in CartItems)
            {
                total += item.product.Price * item.quantity;
            }
            return total;
            
        }
        
        public DateTime CreatedDate { get; set; }
    }
}
