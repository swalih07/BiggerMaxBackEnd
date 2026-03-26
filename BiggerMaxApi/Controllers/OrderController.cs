using Application.DTOs;
using Application.Interfaces;
using BiggerMaxApi.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BiggerMaxApi.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        // CHECKOUT
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(CheckoutDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(ApiResponse<object>.Fail("Invalid user"));

                var result = await _orderService.CheckoutAsync(userId, dto);

                return Ok(
                    ApiResponse<CheckoutResponseDto>
                        .SuccessResponse(result, "Checkout successful")
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ApiResponse<object>.Fail(ex.Message)
                );
            }
        }


        // GET MY ORDERS

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _orderService.GetUserOrdersAsync(userId);

            return Ok(new ApiResponse<List<OrderResponseDto>>
            {
                Success = true,
                Data = result
            });
        }

        
        // CANCEL ORDER
        
        [HttpPut("cancel/{orderId}")]
        public async Task<IActionResult> Cancel(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var success = await _orderService.CancelOrderAsync(userId, orderId);

            return Ok(new ApiResponse<object>
            {
                Success = success,
                Message = success ? "Order cancelled successfully" : "Unable to cancel order"
            });
        }
    }
}
