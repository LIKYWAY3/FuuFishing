namespace ASPtestShop.Models.DTO.Order
{
    public class OrderHistoryDto
    {
        public int OrderId { get; set; }

        public string OrderCode { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal ShippingFee { get; set; }

        public decimal FinalAmount { get; set; }

        public string OrderStatus { get; set; }

        public string PaymentStatus { get; set; }

        public string PaymentMethod { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverPhone { get; set; }

        public string ShippingAddress { get; set; }

        public string? Note { get; set; }

        public string? CancelReason { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<OrderHistoryItemDto> Items { get; set; }
            = new List<OrderHistoryItemDto>();
    }
}