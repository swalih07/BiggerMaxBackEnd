using Application.DTOs;
using Application.Interfaces.AdminInterfaces;
using BiggerMaxApi.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiggerMaxApi.Controllers.AdminControllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/orders")]
    [ApiController]
    public class AdminOrderController : ControllerBase
    {
        private readonly IAdminOrderService _service;

        public AdminOrderController(IAdminOrderService service)
        {
            _service = service;
        }

        //  GET ALL ORDERS
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _service.GetAllOrderItemsAsync();

            return Ok(ApiResponse<object>
                .SuccessResponse(result, "Orders fetched successfully"));
        }

        //  GET ORDER BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var result = await _service.GetOrderByIdAsync(id);

            if (result == null || result.Count == 0)
            {
                return NotFound(ApiResponse<object>
                    .Fail("Order not found"));
            }

            return Ok(ApiResponse<object>
                .SuccessResponse(result, "Order fetched successfully"));
        }

        //  UPDATE ORDER STATUS
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusDto dto)
        {
            try
            {
                var updated = await _service.UpdateOrderStatusAsync(id, dto.Status);

                if (!updated)
                {
                    return NotFound(ApiResponse<object>
                        .Fail("Order not found"));
                }

                return Ok(ApiResponse<object>
                    .SuccessResponse(null, "Order status updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>
                    .Fail(ex.Message));
            }
        }
    }
}