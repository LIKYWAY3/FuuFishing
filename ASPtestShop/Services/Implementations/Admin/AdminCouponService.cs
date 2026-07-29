using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Coupon;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations.Admin
{
    public class AdminCouponService : IAdminCouponService
    {
        private readonly AppDbContext _context;

        public AdminCouponService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminCouponDto>> GetCouponsAsync(string? search = null)
        {
            var query = _context.Coupons.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLower();
                query = query.Where(c => c.Code.ToLower().Contains(keyword));
            }

            return await query
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new AdminCouponDto
                {
                    CouponId = c.CouponId,
                    Code = c.Code,
                    DiscountType = c.DiscountType,
                    DiscountValue = c.DiscountValue,
                    MinOrderAmount = c.MinOrderAmount,
                    MaxDiscountAmount = c.MaxDiscountAmount,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    IsActive = c.IsActive
                })
                .ToListAsync();
        }

        public async Task<AdminCouponDto?> GetCouponByIdAsync(int id)
        {
            var c = await _context.Coupons.FindAsync(id);
            if (c == null) return null;

            return new AdminCouponDto
            {
                CouponId = c.CouponId,
                Code = c.Code,
                DiscountType = c.DiscountType,
                DiscountValue = c.DiscountValue,
                MinOrderAmount = c.MinOrderAmount,
                MaxDiscountAmount = c.MaxDiscountAmount,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                IsActive = c.IsActive
            };
        }

        public async Task<(bool Success, string Message, AdminCouponDto? Coupon)> CreateCouponAsync(CreateCouponDto dto)
        {
            var cleanCode = dto.Code.Trim().ToUpper();
            var exists = await _context.Coupons.AnyAsync(c => c.Code.ToUpper() == cleanCode);
            if (exists)
            {
                return (false, "Mã giảm giá này đã tồn tại.", null);
            }

            if (dto.EndDate <= dto.StartDate)
            {
                return (false, "Ngày kết thúc phải lớn hơn ngày bắt đầu.", null);
            }

            var coupon = new Coupon
            {
                Code = cleanCode,
                DiscountType = dto.DiscountType.ToUpper(),
                DiscountValue = dto.DiscountValue,
                MinOrderAmount = dto.MinOrderAmount,
                MaxDiscountAmount = dto.MaxDiscountAmount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            var result = await GetCouponByIdAsync(coupon.CouponId);
            return (true, "Tạo mã giảm giá thành công!", result);
        }

        public async Task<(bool Success, string Message)> UpdateCouponAsync(int id, UpdateCouponDto dto)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return (false, "Không tìm thấy mã giảm giá.");
            }

            var cleanCode = dto.Code.Trim().ToUpper();
            var exists = await _context.Coupons.AnyAsync(c => c.Code.ToUpper() == cleanCode && c.CouponId != id);
            if (exists)
            {
                return (false, "Mã giảm giá đã trùng với mã khác.");
            }

            coupon.Code = cleanCode;
            coupon.DiscountType = dto.DiscountType.ToUpper();
            coupon.DiscountValue = dto.DiscountValue;
            coupon.MinOrderAmount = dto.MinOrderAmount;
            coupon.MaxDiscountAmount = dto.MaxDiscountAmount;
            coupon.StartDate = dto.StartDate;
            coupon.EndDate = dto.EndDate;
            coupon.IsActive = dto.IsActive;
            coupon.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, "Cập nhật mã giảm giá thành công!");
        }

        public async Task<(bool Success, string Message)> DeleteCouponAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return (false, "Không tìm thấy mã giảm giá.");
            }

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return (true, "Đã xóa mã giảm giá thành công.");
        }
    }
}
