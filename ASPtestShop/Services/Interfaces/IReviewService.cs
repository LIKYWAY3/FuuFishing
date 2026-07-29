using ASPtestShop.Models.DTO.Review;

namespace ASPtestShop.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ProductReviewSummaryDto> GetReviewsByProductIdAsync(int productId);
        Task<(bool Success, string Message, ReviewDto? Review)> AddReviewAsync(string userId, CreateReviewDto dto);
        Task<bool> DeleteReviewAsync(int reviewId, string userId, bool isAdmin = false);
    }
}
