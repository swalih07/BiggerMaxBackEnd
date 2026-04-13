using Application.DTOs;
using Application.Interfaces;
using BiggerMaxApi.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles ="User,Admin")] // Both User and Admin can have a cart
[ApiController]
[Route("api/[controller]")] // Base route: api/Cart
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    // Inject CartService via constructor
    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    // POST: api/Cart/add  → Add item to cart
    [HttpPost("add")]
    public async Task<IActionResult> Add(AddToCartDto dto)
    {
        // Extract logged-in user id from JWT token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Optional safety check (recommended)
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "User not authorized"
            });

        await _cartService.AddToCartAsync(userId, dto); // Call service layer

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Item added to cart"
        });
    }

    // GET: api/Cart  → View cart
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "User not authorized"
            });

        var result = await _cartService.GetCartAsync(userId);

        return Ok(new ApiResponse<CartResponseDto>
        {
            Success = true,
            Data = result
        });
    }

    //  api/Cart/update  → Update quantity
    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateCartDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "User not authorized"
            });

        var success = await _cartService.UpdateCartAsync(userId, dto);

        return Ok(new ApiResponse<object>
        {
            Success = success,
            Message = success ? "Updated" : "Item not found"
        });
    }

    // DELETE: api/Cart/remove/{productId} → Remove item
    [HttpDelete("remove/{productId}")]
    public async Task<IActionResult> Delete(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "User not authorized"
            });

        var success = await _cartService.DeleteCartItemAsync(userId, productId);

        return Ok(new ApiResponse<object>
        {
            Success = success,
            Message = success ? "Deleted" : "Item not found"
        });
    }
}
