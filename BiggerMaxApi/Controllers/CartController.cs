using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiggerMaxApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpPost("add")]
        public async Task<IActionResult>AddToCart(AddToCartDto dto)
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    MessageProcessingHandler = "User Not Authorized"
                });
            }
            await _cartService.AddToCartAsync(userId, dto);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                MessageProcessingHandler = "Product Added to Cart"
            });
        }
    }
}
