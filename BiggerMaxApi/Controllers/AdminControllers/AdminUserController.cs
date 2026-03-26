using Application.DTOs;
using Application.Interfaces.AdminInterfaces;
using BiggerMaxApi.Common;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BiggerMaxApi.Controllers.AdminControllers
{


    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserService _service;

        public AdminUserController(IAdminUserService service)
        {
            _service = service;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _service.GetAllUsersAsync();
            return Ok(ApiResponse<object>.SuccessResponse(users, "Users Fetched Successfully"));
        }

        [HttpGet("users/{id}/details")]
        public async Task<IActionResult> GetUserWithOrders(int id)
        {
            var user = await _service.GetUserWithOrdersAsync(id);

            if (user == null)
                return NotFound(ApiResponse<string>.Fail("User not found"));

            return Ok(ApiResponse<object>.SuccessResponse(user, "User details fetched successfully"));
        }

        [HttpPut("users/{id}/block")]
        public async Task<IActionResult> BlockUser(int id)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var result = await _service.BlockUserAsync(id, adminId);

            if (!result)
                return BadRequest(ApiResponse<string>.Fail("Operation failed"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "User Blocked Successfully"));
        }

        [HttpPut("users/{id}/unblock")]
        public async Task<IActionResult> UnblockUser(int id)
        {
            var result = await _service.UnblockUserAsync(id);

            if (!result)
                return BadRequest(ApiResponse<string>.Fail("User not found"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "User Unblocked Successfully"));
        }

        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> ChangeUserRole(int id, ChangeUserRoleDto dto)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var result = await _service.ChangeUserRoleAsync(id, dto, adminId);

            if (!result)
                return BadRequest(ApiResponse<string>.Fail("Operation failed"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "User role updated successfully"));
        }
        [HttpGet("product/{productId}/orders")]
        public async Task<IActionResult> GetUsersByProduct(int productId)
        {
            var result = await _service.GetUsersByProductIdAsync(productId);

            if (result == null || !result.Any())
            {
                return NotFound(
                    ApiResponse<object>.Fail("No users found for this product")
                );
            }

            return Ok(
                ApiResponse<object>.SuccessResponse(
                    result,
                    "Users fetched successfully"
                )
            );
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var result = await _service.DeleteUserAsync(id, adminId);

            if (!result)
                return BadRequest(ApiResponse<string>.Fail("Operation failed"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "User deleted successfully"));
        }
    }
}
