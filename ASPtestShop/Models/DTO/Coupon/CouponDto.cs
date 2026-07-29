using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.DTO.Coupon
{
    public class ApplyCouponRequestDto
    {
        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        public string Code { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
    }

    public class ApplyCouponResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? CouponId { get; set; }
        public string? Code { get; set; }
        public string? DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }

    public class AdminCouponDto
    {
        public int CouponId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = "PERCENT"; // PERCENT or FIXED
        public decimal DiscountValue { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCouponDto
    {
        [Required(ErrorMessage = "Mã giảm giá không được để trống")]
        [StringLength(50, ErrorMessage = "Mã giảm giá tối đa 50 ký tự")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại giảm giá")]
        public string DiscountType { get; set; } = "PERCENT"; // PERCENT or FIXED

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn 0")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị tối thiểu không hợp lệ")]
        public decimal MinOrderAmount { get; set; } = 0;

        public decimal? MaxDiscountAmount { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);
    }

    public class UpdateCouponDto
    {
        [Required(ErrorMessage = "Mã giảm giá không được để trống")]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string DiscountType { get; set; } = "PERCENT";

        public decimal DiscountValue { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
