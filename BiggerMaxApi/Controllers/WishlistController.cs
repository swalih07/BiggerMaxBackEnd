using Application.DTOs;
using Application.Interfaces;
using BiggerMaxApi.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiggerMaxApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }
        [HttpPost("add")]
        public async Task<ActionResult<ApiResponse<string>>>AddtoWishlist(AddToWishlistDto dto)
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Unauthorized"
                });
            await _wishlistService.AddToWishlistAsync(userId, dto);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message="Product Added To Wishlist"
            });
        }
    }
}
