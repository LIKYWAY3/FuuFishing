namespace ASPtestShop.Models.DTO.Review
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int ProductId { get; set; }
        public string? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserAvatar { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ProductReviewSummaryDto
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public List<ReviewDto> Reviews { get; set; } = new();
    }
}
