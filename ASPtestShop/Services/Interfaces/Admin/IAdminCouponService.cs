using ASPtestShop.Models.DTO.Coupon;

namespace ASPtestShop.Services.Interfaces.Admin
{
    public interface IAdminCouponService
    {
        Task<List<AdminCouponDto>> GetCouponsAsync(string? search = null);
        Task<AdminCouponDto?> GetCouponByIdAsync(int id);
        Task<(bool Success, string Message, AdminCouponDto? Coupon)> CreateCouponAsync(CreateCouponDto dto);
        Task<(bool Success, string Message)> UpdateCouponAsync(int id, UpdateCouponDto dto);
        Task<(bool Success, string Message)> DeleteCouponAsync(int id);
    }
}
