using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Review;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations.Admin
{
    public class AdminReviewService : IAdminReviewService
    {
        private readonly AppDbContext _context;

        public AdminReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminReviewDto>> GetAllReviewsAsync(bool? isApproved = null, string? search = null)
        {
            var query = _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .Where(r => r.IsActive);

            if (isApproved.HasValue)
            {
                query = query.Where(r => r.IsApproved == isApproved.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLower();
                query = query.Where(r =>
                    (r.Comment != null && r.Comment.ToLower().Contains(keyword)) ||
                    (r.Product != null && r.Product.ProductName.ToLower().Contains(keyword)) ||
                    (r.User != null && (r.User.FullName!.ToLower().Contains(keyword) || r.User.UserName!.ToLower().Contains(keyword)))
                );
            }

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new AdminReviewDto
                {
                    ReviewId = r.ReviewId,
                    ProductId = r.ProductId,
                    ProductName = r.Product != null ? r.Product.ProductName : "N/A",
                    ProductImage = r.Product != null ? r.Product.ThumbnailUrl : null,
                    UserId = r.UserId,
                    UserName = r.User != null ? (!string.IsNullOrWhiteSpace(r.User.FullName) ? r.User.FullName : r.User.UserName ?? "N/A") : "Vô danh",
                    UserEmail = r.User != null ? r.User.Email : null,
                    UserAvatar = r.User != null ? r.User.AvatarUrl : null,
                    Rating = r.Rating,
                    Comment = r.Comment ?? string.Empty,
                    IsApproved = r.IsApproved,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> ApproveReviewAsync(int reviewId, bool isApproved)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null || !review.IsActive)
            {
                return (false, "Không tìm thấy bình luận.");
            }

            review.IsApproved = isApproved;
            review.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var statusStr = isApproved ? "duyệt thành công" : "bỏ duyệt (ẩn)";
            return (true, $"Đã {statusStr} bình luận #{reviewId}.");
        }

        public async Task<(bool Success, string Message)> DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null || !review.IsActive)
            {
                return (false, "Không tìm thấy bình luận.");
            }

            review.IsActive = false;
            review.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return (true, $"Đã xóa bình luận #{reviewId} thành công.");
        }
    }
}
