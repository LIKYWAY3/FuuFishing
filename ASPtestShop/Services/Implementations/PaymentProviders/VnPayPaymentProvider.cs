using ASPtestShop.Models.DTO.Payment;
using ASPtestShop.Services.PaymentProviders;
using Microsoft.Extensions.Configuration;

namespace ASPtestShop.Services.Implementations.PaymentProviders
{
    public class VnPayPaymentProvider : IPaymentProvider
    {
        private readonly IConfiguration _configuration;

        public VnPayPaymentProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string PaymentMethod => "VNPAY";

        public Task<CreatePaymentResultDto> CreatePaymentAsync(CreatePaymentRequestDto request)
        {
            var transactionCode = $"VNPAY_{request.OrderCode}_{DateTime.UtcNow:yyyyMMddHHmmss}";
            var paymentUrl = $"/payment/vnpay?orderCode={request.OrderCode}&amount={request.Amount:F0}&transId={transactionCode}";

            var result = new CreatePaymentResultDto
            {
                IsSuccess = true,
                Message = "Khởi tạo thanh toán VNPAY thành công.",
                PaymentMethod = "VNPAY",
                PaymentStatus = "Pending",
                PaymentUrl = paymentUrl,
                TransactionCode = transactionCode,
                RequiresRedirect = true
            };

            return Task.FromResult(result);
        }
    }
}