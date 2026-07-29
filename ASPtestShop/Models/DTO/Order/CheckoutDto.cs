using ASPtestShop.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.DTO.Order
{
    public class CheckoutDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người nhận")]
        [MaxLength(100)]
        public string ReceiverName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại người nhận")]
        [MaxLength(20)]
        public string ReceiverPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [MaxLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Note { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public string? CouponCode { get; set; }

        [MinLength(1, ErrorMessage = "Vui lòng chọn ít nhất một sản phẩm để thanh toán")]
        public List<int> CartItemIds { get; set; } = new();
    }
}