using Application.DTOs;
using Application.Interfaces;
using BiggerMaxApi.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BiggerMaxApi.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class ShippingAddressController : ControllerBase
    {
        private readonly IShippingAddressService _service;

        public ShippingAddressController(IShippingAddressService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddShippingAddressDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _service.AddAsync(userId, dto);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Address added"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _service.GetAllAsync(userId);

            return Ok(ApiResponse<List<ShippingAddressDto>>
                .SuccessResponse(result, result.Any() ? "Addresses fetched" : "No addresses found"));
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateShippingAddressDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _service.UpdateAsync(userId, dto);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Address updated"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _service.DeleteAsync(userId, id);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Address deleted"));
        }
    }

}
