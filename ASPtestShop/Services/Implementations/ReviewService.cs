using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Review;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ProductReviewSummaryDto> GetReviewsByProductIdAsync(int productId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId && r.IsActive && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    UserName = !string.IsNullOrWhiteSpace(r.User != null ? r.User.FullName : null)
                        ? r.User!.FullName!
                        : (r.User != null && !string.IsNullOrWhiteSpace(r.User.UserName) ? r.User.UserName : "Người dùng"),
                    UserAvatar = r.User != null ? r.User.AvatarUrl : null,
                    Rating = r.Rating,
                    Comment = r.Comment ?? string.Empty,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            var total = reviews.Count;
            var avg = total > 0 ? Math.Round(reviews.Average(r => r.Rating), 1) : 0;

            return new ProductReviewSummaryDto
            {
                TotalReviews = total,
                AverageRating = avg,
                Reviews = reviews
            };
        }

        public async Task<(bool Success, string Message, ReviewDto? Review)> AddReviewAsync(string userId, CreateReviewDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return (false, "Bạn cần đăng nhập để thực hiện bình luận.", null);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "Không tìm thấy tài khoản người dùng.", null);
            }

            var productExists = await _context.Products.AnyAsync(p => p.ProductId == dto.ProductId && p.IsActive);
            if (!productExists)
            {
                return (false, "Sản phẩm không tồn tại.", null);
            }

            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return (false, "Đánh giá phải từ 1 đến 5 sao.", null);
            }

            if (string.IsNullOrWhiteSpace(dto.Comment))
            {
                return (false, "Nội dung bình luận không được để trống.", null);
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isApproved = isAdmin; // Nếu là Admin thì tự duyệt, còn người dùng thường thì cần chờ duyệt

            var review = new Review
            {
                ProductId = dto.ProductId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment.Trim(),
                IsApproved = isApproved,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var displayName = !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName : user.UserName ?? "Người dùng";

            var reviewDto = new ReviewDto
            {
                ReviewId = review.ReviewId,
                ProductId = review.ProductId,
                UserId = review.UserId,
                UserName = displayName,
                UserAvatar = user.AvatarUrl,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };

            var msg = isApproved
                ? "Đã gửi bình luận thành công!"
                : "Cảm ơn bạn! Bình luận đã được gửi và đang chờ quản trị viên kiểm duyệt.";

            return (true, msg, reviewDto);
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, string userId, bool isAdmin = false)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null || !review.IsActive)
            {
                return false;
            }

            if (!isAdmin && review.UserId != userId)
            {
                return false;
            }

            review.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
