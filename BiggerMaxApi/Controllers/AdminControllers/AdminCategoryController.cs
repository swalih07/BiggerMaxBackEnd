using Application.DTOs;
using Application.Interfaces.AdminInterfaces;
using BiggerMaxApi.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiggerMaxApi.Controllers.AdminControllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/categories")]
    public class AdminCategoryController : ControllerBase
    {
        private readonly IAdminCategoryService _service;

        public AdminCategoryController(IAdminCategoryService service)
        {
            _service = service;
        }

        //  CREATE
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequestDto request)
        {
            var result = await _service.CreateCategoryAsync(request);

            return Ok(
                ApiResponse<CategoryDto>.SuccessResponse(
                    result,
                    "Category created successfully"
                )
            );
        }

        //  GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllCategoriesAsync();

            if (result == null || !result.Any())
            {
                return NotFound(
                    ApiResponse<List<CategoryDto>>
                        .Fail("No categories found")
                );
            }

            return Ok(
                ApiResponse<List<CategoryDto>>
                    .SuccessResponse(result, "Categories fetched successfully")
            );
        }

        //  UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCategoryDto dto)
        {
            try
            {
                var updated = await _service.UpdateCategoryAsync(id, dto);

                if (!updated)
                {
                    return NotFound(
                        ApiResponse<object>.Fail("Category not found")
                    );
                }

                return Ok(
                    ApiResponse<object>.SuccessResponse(
                        null,
                        "Category updated successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ApiResponse<object>.Fail(ex.Message)
                );
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteCategoryAsync(id);

            if (!deleted)
            {
                return NotFound(
                    ApiResponse<object>.Fail("Category not found")
                );
            }

            return Ok(
                ApiResponse<object>.SuccessResponse(
                    null,
                    "Category deleted successfully"
                )
            );
        }
    }
}