using ASPtestShop.Models.DTO.Payment;
using ASPtestShop.Services.PaymentProviders;
using Microsoft.Extensions.Configuration;

namespace ASPtestShop.Services.Implementations.PaymentProviders
{
    public class MomoPaymentProvider : IPaymentProvider
    {
        private readonly IConfiguration _configuration;

        public MomoPaymentProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string PaymentMethod => "MOMO";

        public Task<CreatePaymentResultDto> CreatePaymentAsync(CreatePaymentRequestDto request)
        {
            var partnerCode = _configuration["Payment:MoMo:PartnerCode"];
            var transactionCode = $"MOMO_{request.OrderCode}_{DateTime.UtcNow:yyyyMMddHHmmss}";

            // URL chuyển hướng người dùng đến trang thanh toán MoMo (hỗ trợ Sandbox/Mô phỏng)
            var paymentUrl = $"/payment/momo?orderCode={request.OrderCode}&amount={request.Amount:F0}&transId={transactionCode}";

            var result = new CreatePaymentResultDto
            {
                IsSuccess = true,
                Message = "Khởi tạo thanh toán MoMo thành công.",
                PaymentMethod = "MOMO",
                PaymentStatus = "Pending",
                PaymentUrl = paymentUrl,
                TransactionCode = transactionCode,
                RequiresRedirect = true
            };

            return Task.FromResult(result);
        }
    }
}