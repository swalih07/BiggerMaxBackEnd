using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BiggerMaxApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("pay/{orderId}")]
        public async Task<IActionResult> Pay(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _paymentService.PayAsync(userId!, orderId);

            return Ok(result);
        }
        [HttpPost("verify-payment")]
        public IActionResult VerifyPayment([FromBody] RazorpayVerifyDto model)
        {
            bool isValid = _paymentService.VerifyPayment(model);

            if (!isValid)
                return BadRequest("Invalid Signature");

            return Ok("Payment Verified");
        }
    }

}
