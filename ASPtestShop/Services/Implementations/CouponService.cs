using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Coupon;
using ASPtestShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations
{
    public class CouponService : ICouponService
    {
        private readonly AppDbContext _context;

        public CouponService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApplyCouponResultDto> ApplyCouponAsync(ApplyCouponRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return new ApplyCouponResultDto
                {
                    Success = false,
                    Message = "Vui lòng nhập mã giảm giá."
                };
            }

            var cleanCode = request.Code.Trim().ToUpper();
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == cleanCode && c.IsActive);

            if (coupon == null)
            {
                return new ApplyCouponResultDto
                {
                    Success = false,
                    Message = "Mã giảm giá không tồn tại hoặc đã bị khóa."
                };
            }

            var now = DateTime.UtcNow;
            if (now < coupon.StartDate)
            {
                return new ApplyCouponResultDto
                {
                    Success = false,
                    Message = $"Mã giảm giá chỉ có hiệu lực từ ngày {coupon.StartDate:dd/MM/yyyy}."
                };
            }

            if (now > coupon.EndDate)
            {
                return new ApplyCouponResultDto
                {
                    Success = false,
                    Message = "Mã giảm giá đã hết hạn sử dụng."
                };
            }

            if (request.TotalAmount < coupon.MinOrderAmount)
            {
                var minAmountStr = string.Format(new System.Globalization.CultureInfo("vi-VN"), "{0:C0}", coupon.MinOrderAmount);
                return new ApplyCouponResultDto
                {
                    Success = false,
                    Message = $"Đơn hàng tối thiểu phải từ {minAmountStr} để áp dụng mã giảm giá này."
                };
            }

            decimal discountAmount = 0;
            var isPercent = string.Equals(coupon.DiscountType, "PERCENT", StringComparison.OrdinalIgnoreCase);

            if (isPercent)
            {
                discountAmount = request.TotalAmount * (coupon.DiscountValue / 100m);
                if (coupon.MaxDiscountAmount.HasValue && coupon.MaxDiscountAmount.Value > 0)
                {
                    discountAmount = Math.Min(discountAmount, coupon.MaxDiscountAmount.Value);
                }
            }
            else
            {
                discountAmount = coupon.DiscountValue;
            }

            discountAmount = Math.Min(discountAmount, request.TotalAmount); // Không giảm quá tổng tiền
            var finalAmount = Math.Max(request.TotalAmount - discountAmount, 0);

            return new ApplyCouponResultDto
            {
                Success = true,
                Message = $"Áp dụng mã giảm giá '{coupon.Code}' thành công!",
                CouponId = coupon.CouponId,
                Code = coupon.Code,
                DiscountType = coupon.DiscountType,
                DiscountValue = coupon.DiscountValue,
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount
            };
        }
    }
}
