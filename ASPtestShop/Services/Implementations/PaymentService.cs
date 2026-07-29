using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Payment;
using ASPtestShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations
{
    // PaymentService xử lý các nghiệp vụ sau khi payment đã được tạo
    // Ví dụ: lấy thông tin payment, xác nhận COD
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        // Lấy thông tin thanh toán theo OrderId
        public async Task<PaymentResultDto?> GetPaymentByOrderIdAsync(int orderId, string userId)
        {
            var payment = await _context.Payments
                // Chỉ lấy payment thuộc đơn hàng của user đang đăng nhập
                .Where(p => p.OrderId == orderId && p.Order.UserId == userId)

                // Map sang DTO, không trả entity Payment trực tiếp
                .Select(p => new PaymentResultDto
                {
                    PaymentId = p.PaymentId,
                    OrderId = p.OrderId,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus,
                    TransactionCode = p.TransactionCode,
                    PaidAt = p.PaidAt,

                    Order = new PaymentOrderDto
                    {
                        OrderId = p.Order.OrderId,
                        OrderCode = p.Order.OrderCode,
                        FinalAmount = p.Order.FinalAmount,
                        OrderStatus = p.Order.OrderStatus,
                        PaymentStatus = p.Order.PaymentStatus
                    }
                })
                .FirstOrDefaultAsync();

            return payment;
        }

        // Xác nhận thanh toán COD
        public async Task<bool> ConfirmCodPaymentAsync(int orderId, string userId)
        {
            var payment = await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p =>
                    p.OrderId == orderId &&
                    p.Order.UserId == userId);

            if (payment == null) return false;

            if (!string.Equals(payment.PaymentMethod, "COD", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (string.Equals(payment.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            payment.PaymentStatus = "Paid";
            payment.PaidAt = DateTime.Now;
            payment.Order.PaymentStatus = "Paid";

            await _context.SaveChangesAsync();
            return true;
        }

        // Xác nhận thanh toán Online (MoMo, ZaloPay, VNPay)
        public async Task<bool> ConfirmOnlinePaymentAsync(string orderCode, string paymentMethod, string? transactionCode = null)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
            if (order == null) return false;

            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == order.OrderId);
            if (payment == null)
            {
                payment = new Payment
                {
                    OrderId = order.OrderId,
                    PaymentMethod = paymentMethod,
                    PaymentStatus = "Paid",
                    TransactionCode = transactionCode,
                    PaidAt = DateTime.Now
                };
                _context.Payments.Add(payment);
            }
            else
            {
                payment.PaymentStatus = "Paid";
                payment.PaidAt = DateTime.Now;
                if (!string.IsNullOrEmpty(transactionCode))
                {
                    payment.TransactionCode = transactionCode;
                }
            }

            order.PaymentStatus = "Paid";
            order.OrderStatus = "Processing";

            await _context.SaveChangesAsync();
            return true;
        }
    }
}