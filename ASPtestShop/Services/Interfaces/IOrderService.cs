using ASPtestShop.Models.DTO.Order;
using ASPtestShop.Models.ViewModels.Profile;

namespace ASPtestShop.Services.Interfaces
{
    public interface IOrderService
    {
        Task<CheckoutResultDto> CheckoutAsync(string userId, CheckoutDto checkoutDto);

        Task<List<OrderHistoryDto>> GetOrderHistoryAsync(string userId);

        Task<OrderHistoryDto?> GetOrderDetailAsync(string userId, int orderId);

        Task<(bool Success, string Message)> CancelOrderAsync(string userId, int orderId, string? reason);

    }
}