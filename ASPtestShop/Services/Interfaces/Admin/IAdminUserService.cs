using ASPtestShop.Models.DTO.User;

namespace ASPtestShop.Services.Interfaces.Admin
{
    public interface IAdminUserService
    {
        Task<List<AdminUserDto>> GetUsersAsync(string? search = null, string? role = null);
        Task<AdminUserDto?> GetUserByIdAsync(string userId);
        Task<(bool Success, string Message, AdminUserDto? User)> CreateUserAsync(CreateAdminUserDto dto);
        Task<(bool Success, string Message)> UpdateUserAsync(string userId, UpdateAdminUserDto dto);
        Task<(bool Success, string Message)> ToggleLockUserAsync(string userId, string currentAdminId);
        Task<(bool Success, string Message)> ResetPasswordAsync(string userId, string newPassword);
        Task<(bool Success, string Message)> DeleteUserAsync(string userId, string currentAdminId);
    }
}
