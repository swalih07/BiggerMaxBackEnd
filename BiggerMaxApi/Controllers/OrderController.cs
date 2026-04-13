using Application.DTOs;
using Application.Interfaces;
using BiggerMaxApi.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BiggerMaxApi.Controllers
{
    [Authorize(Roles = "User,Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        // PLACE FULL CART ORDER
        [HttpPost("place-cart-order")]
        public async Task<IActionResult> PlaceCartOrder(PlaceOrderDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new ApiResponse<string> { Success = false, Message = "User not found" });

                var result = await _orderService.PlaceCartOrderAsync(userId, dto);

                return Ok(new ApiResponse<OrderResponseDto>
                {
                    Success = true,
                    Message = "Order placed successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = ex.Message });
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
