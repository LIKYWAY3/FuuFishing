using ASPtestShop.Models.DTO.Coupon;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api
{
    [Route("api/coupons")]
    [ApiController]
    public class CouponApiController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponApiController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        // POST: api/coupons/apply
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _couponService.ApplyCouponAsync(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
