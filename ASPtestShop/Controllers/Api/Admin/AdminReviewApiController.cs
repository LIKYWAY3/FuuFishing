using ASPtestShop.Auth;
using ASPtestShop.Models.DTO.Review;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api.Admin
{
    [Route("api/admin/reviews")]
    [ApiController]
    [Authorize(
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + AdminCookieAuth.Scheme,
        Roles = "Admin"
    )]
    public class AdminReviewApiController : ControllerBase
    {
        private readonly IAdminReviewService _adminReviewService;

        public AdminReviewApiController(IAdminReviewService adminReviewService)
        {
            _adminReviewService = adminReviewService;
        }

        // GET: api/admin/reviews?isApproved=true/false&search=keyword
        [HttpGet]
        public async Task<IActionResult> GetReviews([FromQuery] bool? isApproved, [FromQuery] string? search)
        {
            var reviews = await _adminReviewService.GetAllReviewsAsync(isApproved, search);
            return Ok(reviews);
        }

        // PUT: api/admin/reviews/{reviewId}/approve
        [HttpPut("{reviewId:int}/approve")]
        public async Task<IActionResult> ApproveReview(int reviewId, [FromBody] UpdateReviewApprovalDto dto)
        {
            var (success, message) = await _adminReviewService.ApproveReviewAsync(reviewId, dto.IsApproved);
            if (!success)
            {
                return BadRequest(new { Success = false, Message = message });
            }

            return Ok(new { Success = true, Message = message });
        }

        // DELETE: api/admin/reviews/{reviewId}
        [HttpDelete("{reviewId:int}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var (success, message) = await _adminReviewService.DeleteReviewAsync(reviewId);
            if (!success)
            {
                return BadRequest(new { Success = false, Message = message });
            }

            return Ok(new { Success = true, Message = message });
        }
    }
}
