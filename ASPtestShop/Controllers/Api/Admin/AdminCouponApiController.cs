using ASPtestShop.Auth;
using ASPtestShop.Models.DTO.Coupon;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api.Admin
{
    [Route("api/admin/coupons")]
    [ApiController]
    [Authorize(
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + AdminCookieAuth.Scheme,
        Roles = "Admin"
    )]
    public class AdminCouponApiController : ControllerBase
    {
        private readonly IAdminCouponService _adminCouponService;

        public AdminCouponApiController(IAdminCouponService adminCouponService)
        {
            _adminCouponService = adminCouponService;
        }

        // GET: api/admin/coupons?search=abc
        [HttpGet]
        public async Task<IActionResult> GetCoupons([FromQuery] string? search)
        {
            var coupons = await _adminCouponService.GetCouponsAsync(search);
            return Ok(coupons);
        }

        // GET: api/admin/coupons/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCouponById(int id)
        {
            var coupon = await _adminCouponService.GetCouponByIdAsync(id);
            if (coupon == null) return NotFound(new { Message = "Không tìm thấy mã giảm giá." });
            return Ok(coupon);
        }

        // POST: api/admin/coupons
        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, message, coupon) = await _adminCouponService.CreateCouponAsync(dto);
            if (!success)
            {
                return BadRequest(new { Success = false, Message = message });
            }

            return Ok(new { Success = true, Message = message, Coupon = coupon });
        }

        // PUT: api/admin/coupons/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCoupon(int id, [FromBody] UpdateCouponDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, message) = await _adminCouponService.UpdateCouponAsync(id, dto);
            if (!success)
            {
                return BadRequest(new { Success = false, Message = message });
            }

            return Ok(new { Success = true, Message = message });
        }

        // DELETE: api/admin/coupons/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var (success, message) = await _adminCouponService.DeleteCouponAsync(id);
            if (!success)
            {
                return BadRequest(new { Success = false, Message = message });
            }

            return Ok(new { Success = true, Message = message });
        }
    }
}
