using System.Security.Claims;
using ASPtestShop.Auth;
using ASPtestShop.Models.DTO.User;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api.Admin
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + AdminCookieAuth.Scheme,
        Roles = "Admin"
    )]
    public class AdminUserApiController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUserApiController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        // GET: api/admin/users?search=abc&role=Admin
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] string? search, [FromQuery] string? role)
        {
            var users = await _adminUserService.GetUsersAsync(search, role);
            return Ok(users);
        }

        // GET: api/admin/users/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _adminUserService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "Không tìm thấy người dùng." });
            }
            return Ok(user);
        }

        // POST: api/admin/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateAdminUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, message, user) = await _adminUserService.CreateUserAsync(dto);
            if (!success)
            {
                return BadRequest(new { Success = false, Message = message });
            }

            return Ok(new { Success = true, Message = message, User = user });
        }

        // PUT: api/admin/users/{userId}
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateAdminUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, message) = await _adminUserService.UpdateUserAsync(userId, dto);
            if (!success)
            {
                return BadRequest(new { Success = false, Message = message });
            }

            return Ok(new { Success = true, Message = message });
        }

        // PUT: api/admin/users/{userId}/lock
        [HttpPut("{userId}/lock")]
        public async Task<IActionResult> ToggleLockUser(string userId)
        {
            var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var (success, message) = await _adminUserService.ToggleLockUserAsync(userId, currentAdminId);

            if (!success)
            {
                return BadRequest(new { Success = false, Message = message });
            }

            return Ok(new { Success = true, Message = message });
        }

        // PUT: api/admin/users/{userId}/reset-password
        [HttpPut("{userId}/reset-password")]
        public async Task<IActionResult> ResetPassword(string userId, [FromBody] AdminResetUserPasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, message) = await _adminUserService.ResetPasswordAsync(userId, dto.NewPassword);
            if (!success)
            {
                return BadRequest(new { Success = false, Message = message });
            }

            return Ok(new { Success = true, Message = message });
        }

        // DELETE: api/admin/users/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var (success, message) = await _adminUserService.DeleteUserAsync(userId, currentAdminId);

            if (!success)
            {
                return BadRequest(new { Success = false, Message = message });
            }

            return Ok(new { Success = true, Message = message });
        }
    }
}
