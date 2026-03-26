using Application.DTOs;
using Application.Interfaces;
using BiggerMaxApi.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles = "User")]
[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [HttpPost("toggle")]
    public async Task<IActionResult> ToggleWishlist(AddToWishlistDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid user"));

            await _wishlistService.ToggleWishlistAsync(userId, dto);

            return Ok(
                ApiResponse<bool>.SuccessResponse(true, "Wishlist updated")
            );
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<object>.Fail(ex.Message)
            );
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetWishlist()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid user"));

            var result = await _wishlistService.GetWishlistAsync(userId);

            if (result == null || !result.Any())
            {
                return Ok(
                    ApiResponse<List<WishlistDto>>
                        .SuccessResponse(new List<WishlistDto>(), "Wishlist is empty")
                );
            }

            return Ok(
                ApiResponse<List<WishlistDto>>
                    .SuccessResponse(result, "Wishlist fetched successfully")
            );
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<object>.Fail(ex.Message)
            );
        }
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearWishlist()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid user"));

            await _wishlistService.ClearWishlistAsync(userId);

            return Ok(
                ApiResponse<bool>.SuccessResponse(true, "Wishlist cleared")
            );
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<object>.Fail(ex.Message)
            );
        }
    }
}
