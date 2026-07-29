namespace ASPtestShop.Models.DTO.Order
{
    public class CheckoutResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public int? OrderId { get; set; }

        public string? OrderCode { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal ShippingFee { get; set; }

        public decimal FinalAmount { get; set; }

        public string? OrderStatus { get; set; }

        public string? PaymentStatus { get; set; }

        public string? PaymentMethod { get; set; }

        public string? PaymentUrl { get; set; }

        public bool RequiresRedirect { get; set; }
    }
}