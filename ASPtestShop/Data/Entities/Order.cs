using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ASPtestShop.Data;
namespace ASPtestShop.Data.Entities
{
    public class Order : BaseEntity
    {
        public int OrderId { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderCode { get; set; } = String.Empty;
        public int? CouponId { get; set; }
        //viết hoa chữ cái đầu (PascalCase) theo chuẩn của .NET:
        public Coupon? Coupon { get; set; }

        public string? UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalAmount { get; set; }

        [MaxLength(30)]
        public string OrderStatus { get; set; } = "Pending";

        [MaxLength(30)]
        public string PaymentStatus { get; set; } = "Unpaid";

        [MaxLength(30)]
        public string PaymentMethod { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ReceiverName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string ReceiverPhone { get; set; } = string.Empty;

        [MaxLength(255)]
        public string ShippingAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Note { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? CancelReason { get; set; }

        public ApplicationUser? User { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();

        public Payment? Payment { get; set; }
    }
}