using System.Security.Claims;
using ASPtestShop.Auth;
using ASPtestShop.Models.DTO.Review;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewApiController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewApiController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // ==================== GET REVIEWS BY PRODUCT ====================
        // GET: api/reviews/product/{productId}
        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetReviewsByProduct(int productId)
        {
            var result = await _reviewService.GetReviewsByProductIdAsync(productId);
            return Ok(result);
        }

        // ==================== ADD REVIEW ====================
        // POST: api/reviews
        // Chỉ cho phép user đã đăng nhập bình luận
        [HttpPost]
        [Authorize(AuthenticationSchemes = UserCookieAuth.Scheme)]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "Bạn chưa đăng nhập. Vui lòng đăng nhập để bình luận!"
                });
            }

            var (success, message, review) = await _reviewService.AddReviewAsync(userId, dto);

            if (!success)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = message
                });
            }

            return Ok(new
            {
                Success = true,
                Message = message,
                Data = review
            });
        }

        // ==================== DELETE REVIEW ====================
        // DELETE: api/reviews/{reviewId}
        [HttpDelete("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = UserCookieAuth.Scheme)]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Success = false, Message = "Bạn chưa đăng nhập." });
            }

            var result = await _reviewService.DeleteReviewAsync(reviewId, userId, isAdmin);
            if (!result)
            {
                return BadRequest(new { Success = false, Message = "Không thể xóa bình luận này." });
            }

            return Ok(new { Success = true, Message = "Xóa bình luận thành công." });
        }
    }
}
