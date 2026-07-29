using ASPtestShop.Models.DTO.Payment;
using ASPtestShop.Services.PaymentProviders;
using Microsoft.Extensions.Configuration;

namespace ASPtestShop.Services.Implementations.PaymentProviders
{
    public class ZaloPayPaymentProvider : IPaymentProvider
    {
        private readonly IConfiguration _configuration;

        public ZaloPayPaymentProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string PaymentMethod => "ZALOPAY";

        public Task<CreatePaymentResultDto> CreatePaymentAsync(CreatePaymentRequestDto request)
        {
            var transactionCode = $"ZALOPAY_{request.OrderCode}_{DateTime.UtcNow:yyyyMMddHHmmss}";
            var paymentUrl = $"/payment/zalopay?orderCode={request.OrderCode}&amount={request.Amount:F0}&transId={transactionCode}";

            var result = new CreatePaymentResultDto
            {
                IsSuccess = true,
                Message = "Khởi tạo thanh toán ZaloPay thành công.",
                PaymentMethod = "ZALOPAY",
                PaymentStatus = "Pending",
                PaymentUrl = paymentUrl,
                TransactionCode = transactionCode,
                RequiresRedirect = true
            };

            return Task.FromResult(result);
        }
    }
}