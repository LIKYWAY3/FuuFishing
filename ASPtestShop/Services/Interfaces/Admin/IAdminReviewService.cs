using ASPtestShop.Models.DTO.Review;

namespace ASPtestShop.Services.Interfaces.Admin
{
    public interface IAdminReviewService
    {
        Task<List<AdminReviewDto>> GetAllReviewsAsync(bool? isApproved = null, string? search = null);
        Task<(bool Success, string Message)> ApproveReviewAsync(int reviewId, bool isApproved);
        Task<(bool Success, string Message)> DeleteReviewAsync(int reviewId);
    }
}
