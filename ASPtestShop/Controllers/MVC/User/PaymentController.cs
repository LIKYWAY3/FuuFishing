using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.MVC.User
{
    [Route("payment")]
    public class PaymentController : Controller
    {
        // GET: /payment/momo?orderCode=OD123&amount=500000&transId=MOMO_123
        [HttpGet("momo")]
        public IActionResult Momo(string orderCode, decimal amount, string? transId)
        {
            ViewBag.OrderCode = orderCode;
            ViewBag.Amount = amount;
            ViewBag.TransId = transId;
            return View();
        }

        // GET: /payment/zalopay?orderCode=OD123&amount=500000&transId=ZALOPAY_123
        [HttpGet("zalopay")]
        public IActionResult ZaloPay(string orderCode, decimal amount, string? transId)
        {
            ViewBag.OrderCode = orderCode;
            ViewBag.Amount = amount;
            ViewBag.TransId = transId;
            return View();
        }

        // GET: /payment/vnpay?orderCode=OD123&amount=500000&transId=VNPAY_123
        [HttpGet("vnpay")]
        public IActionResult VnPay(string orderCode, decimal amount, string? transId)
        {
            ViewBag.OrderCode = orderCode;
            ViewBag.Amount = amount;
            ViewBag.TransId = transId;
            return View();
        }
    }
}
