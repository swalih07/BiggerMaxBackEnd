using Application.DTOs;
using Application.Interfaces.AdminInterfaces;
using BiggerMaxApi.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiggerMaxApi.Controllers.AdminControllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/dashboard")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _service;

        public AdminDashboardController(IAdminDashboardService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _service.GetDashboardDataAsync();

            if (result == null)
            {
                return NotFound(
                    ApiResponse<AdminDashboardDto>
                        .Fail("Dashboard data not available")
                );
            }

            return Ok(
                ApiResponse<AdminDashboardDto>
                    .SuccessResponse(result, "Dashboard fetched successfully")
            );
        }
    }
}