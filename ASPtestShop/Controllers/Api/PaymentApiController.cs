using System.Security.Claims;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api
{
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentApiController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentApiController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        //===============================GET PAYMENT BY ORDERID======================================
        // GET: api/payments/order/{orderId}
        // Lấy thông tin thanh toán theo OrderId
        [HttpGet("order/{orderId:int}")]
        public async Task<IActionResult> GetPaymentByOrderId(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }

            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId, userId);

            if (payment == null)
            {
                return NotFound(new
                {
                    Message = "Không tìm thấy thông tin thanh toán"
                });
            }

            return Ok(new
            {
                Message = "Lấy thông tin thanh toán thành công",
                Payment = payment
            });
        }

        //===============================CONFIRM COD PAYMENT======================================
        // PUT: api/payments/cod/{orderId}/confirm
        // Xác nhận thanh toán COD
        [HttpPut("cod/{orderId:int}/confirm")]
        public async Task<IActionResult> ConfirmCodPayment(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }

            var result = await _paymentService.ConfirmCodPaymentAsync(orderId, userId);

            if (!result)
            {
                return BadRequest(new
                {
                    Message = "Không thể xác nhận thanh toán COD"
                });
            }

            return Ok(new
            {
                Message = "Xác nhận thanh toán COD thành công"
            });
        }

        //===============================CONFIRM ONLINE PAYMENT======================================
        // POST: api/payments/online/confirm
        [AllowAnonymous]
        [HttpPost("online/confirm")]
        public async Task<IActionResult> ConfirmOnlinePayment([FromBody] ConfirmOnlinePaymentRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.OrderCode))
            {
                return BadRequest(new { Success = false, Message = "Mã đơn hàng không hợp lệ" });
            }

            var result = await _paymentService.ConfirmOnlinePaymentAsync(dto.OrderCode, dto.PaymentMethod, dto.TransactionCode);

            if (!result)
            {
                return BadRequest(new { Success = false, Message = "Xác nhận thanh toán thất bại" });
            }

            return Ok(new { Success = true, Message = "Thanh toán thành công!" });
        }
    }

    public class ConfirmOnlinePaymentRequestDto
    {
        public string OrderCode { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TransactionCode { get; set; }
    }
}