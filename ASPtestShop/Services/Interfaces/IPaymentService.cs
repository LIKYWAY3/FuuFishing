using ASPtestShop.Models.DTO.Payment;

namespace ASPtestShop.Services.Interfaces;

public interface IPaymentService
{
    // Lấy thông tin thanh toán theo orderId và userId
    Task<PaymentResultDto?> GetPaymentByOrderIdAsync(int orderId, string userId);

    // Xác nhận thanh toán COD
    Task<bool> ConfirmCodPaymentAsync(int orderId, string userId);

    // Xác nhận thanh toán Online (MoMo, ZaloPay, VNPay)
    Task<bool> ConfirmOnlinePaymentAsync(string orderCode, string paymentMethod, string? transactionCode = null);
}