using ASPtestShop.Models.DTO.Coupon;

namespace ASPtestShop.Services.Interfaces
{
    public interface ICouponService
    {
        Task<ApplyCouponResultDto> ApplyCouponAsync(ApplyCouponRequestDto request);
    }
}
