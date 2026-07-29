using ASPtestShop.Auth;
using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Order;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASPtestShop.Controllers.Api
{
    [ApiController]
    [Route("api/orders")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + UserCookieAuth.Scheme)]
    public class OrderApiController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderApiController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        //===============================CHECKOUT======================================
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
        {
            //xác thực người dùng bản này để bắt chuỗi rỗng nếu chưa đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }
            var result = await _orderService.CheckoutAsync(userId, checkoutDto);
            if (!result.Success)
            {
                return BadRequest(new
                {
                    result.Message
                });
            }

            return Ok(result);
        }

        //===============================GET ORDER HISTORY======================================
        //Lịch sử đơn hàng
        [HttpGet("history")]
        public async Task<IActionResult> GetOrderHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }

            var orders = await _orderService.GetOrderHistoryAsync(userId);

            if (!orders.Any())
            {
                return Ok(new
                {
                    Message = "Bạn chưa có đơn hàng nào",
                    Orders = orders
                });
            }

            return Ok(new
            {
                Message = "Lấy lịch sử đơn hàng thành công",
                Orders = orders
            });
        }

        //===============================GET ORDER DETAILS======================================
        // Lấy chi tiết lịch sử đơn hàng
        [Authorize]
        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }
            // Lấy chi tiết đơn hàng dựa trên orderId và userId
            var order = await _orderService.GetOrderDetailAsync(userId, orderId);
            if (order == null)
            {
                return NotFound(new
                {
                    Message = "Đơn hàng không tồn tại hoặc bạn không có quyền truy cập"
                });
            }
            return Ok(new
            {
                Message = "Lấy chi tiết đơn hàng thành công",
                OrderDetails = order
            });
        }

        //===============================UC-15: CANCEL ORDER======================================
        // POST: api/orders/{id}/cancel
        [HttpPost("{id:int}/cancel")]
        [HttpPost("cancel/{id:int}")]
        public async Task<IActionResult> CancelOrder(int id, [FromBody] CancelOrderDto? dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Success = false, Message = "Bạn chưa đăng nhập" });
            }

            var reason = dto?.Reason;
            var (success, message) = await _orderService.CancelOrderAsync(userId, id, reason);

            if (!success)
            {
                return BadRequest(new { Success = false, Message = message });
            }

            return Ok(new { Success = true, Message = message });
        }
    }
}