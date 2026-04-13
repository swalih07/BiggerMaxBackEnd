using Application.DTOs;
using Application.Interfaces;
using BiggerMaxApi.Common;
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
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new ApiResponse<string> { Success = false, Message = "User ID missing" });

                var result = await _paymentService.PayAsync(userId, orderId);

                return Ok(new ApiResponse<PaymentResponseDto>
                {
                    Success = true,
                    Message = "Razorpay Order created",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("verify-payment")]
        public async Task<IActionResult> VerifyPayment([FromBody] RazorpayVerifyDto model)
        {
            try
            {
                bool isValid = await _paymentService.VerifyAndConfirmPaymentAsync(model);

                if (!isValid)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Invalid signature or Order not found",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Payment confirmed and Order updated",
                    Data = "Success"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }
    }
}
