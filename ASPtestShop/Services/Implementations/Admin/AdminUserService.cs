using ASPtestShop.Data;
using ASPtestShop.Models.DTO.User;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminUserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<AdminUserDto>> GetUsersAsync(string? search = null, string? role = null)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLower();
                query = query.Where(u =>
                    (u.Email != null && u.Email.ToLower().Contains(keyword)) ||
                    (u.UserName != null && u.UserName.ToLower().Contains(keyword)) ||
                    (u.FullName != null && u.FullName.ToLower().Contains(keyword)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(keyword))
                );
            }

            var users = await query.OrderBy(u => u.Email).ToListAsync();
            var userDtos = new List<AdminUserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (!string.IsNullOrWhiteSpace(role) && !roles.Contains(role, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                var isLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow;

                userDtos.Add(new AdminUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    AvatarUrl = user.AvatarUrl,
                    Gender = user.Gender,
                    Roles = roles.ToList(),
                    IsLockedOut = isLocked,
                    LockoutEnd = user.LockoutEnd
                });
            }

            return userDtos;
        }

        public async Task<AdminUserDto?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var isLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow;

            return new AdminUserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                AvatarUrl = user.AvatarUrl,
                Gender = user.Gender,
                Roles = roles.ToList(),
                IsLockedOut = isLocked,
                LockoutEnd = user.LockoutEnd
            };
        }

        public async Task<(bool Success, string Message, AdminUserDto? User)> CreateUserAsync(CreateAdminUserDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return (false, "Email này đã được sử dụng.", null);
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName.Trim(),
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Tạo tài khoản thất bại: {errors}", null);
            }

            var roleName = string.Equals(dto.Role, "Admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Customer";
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
            await _userManager.AddToRoleAsync(user, roleName);

            var createdDto = await GetUserByIdAsync(user.Id);
            return (true, "Tạo tài khoản thành công!", createdDto);
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(string userId, UpdateAdminUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "Không tìm thấy người dùng.");
            }

            user.FullName = dto.FullName.Trim();
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;
            user.Gender = dto.Gender;

            if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailCheck = await _userManager.FindByEmailAsync(dto.Email);
                if (emailCheck != null && emailCheck.Id != user.Id)
                {
                    return (false, "Email đã tồn tại ở tài khoản khác.");
                }
                user.Email = dto.Email;
                user.UserName = dto.Email;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return (false, $"Cập nhật thất bại: {errors}");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var targetRole = string.Equals(dto.Role, "Admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Customer";

            if (!currentRoles.Contains(targetRole))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!await _roleManager.RoleExistsAsync(targetRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole(targetRole));
                }
                await _userManager.AddToRoleAsync(user, targetRole);
            }

            return (true, "Cập nhật thông tin tài khoản thành công.");
        }

        public async Task<(bool Success, string Message)> ToggleLockUserAsync(string userId, string currentAdminId)
        {
            if (userId == currentAdminId)
            {
                return (false, "Bạn không thể khóa tài khoản của chính mình!");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "Không tìm thấy tài khoản.");
            }

            var isCurrentlyLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow;

            if (isCurrentlyLocked)
            {
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);
                return (true, $"Đã mở khóa tài khoản {user.Email}.");
            }
            else
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                return (true, $"Đã khóa tài khoản {user.Email}.");
            }
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "Không tìm thấy tài khoản.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Đặt lại mật khẩu thất bại: {errors}");
            }

            // Đổi SecurityStamp để invalid token cũ của user
            await _userManager.UpdateSecurityStampAsync(user);

            return (true, $"Đã đặt lại mật khẩu mới cho tài khoản {user.Email}.");
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(string userId, string currentAdminId)
        {
            if (userId == currentAdminId)
            {
                return (false, "Bạn không thể xóa tài khoản của chính mình!");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "Không tìm thấy tài khoản.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Xóa tài khoản thất bại: {errors}");
            }

            return (true, "Đã xóa tài khoản người dùng thành công.");
        }
    }
}
