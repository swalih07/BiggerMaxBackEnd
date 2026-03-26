using Application.DTOs;
using Application.Interfaces.AdminInterfaces;
using BiggerMaxApi.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiggerMaxApi.Controllers.AdminControllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/products")]
    public class AdminProductController : ControllerBase
    {
        private readonly IAdminProductService _service;

        public AdminProductController(IAdminProductService service)
        {
            _service = service;
        }

        //  Add Product
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductRequest request)
        {
            if (request.Image == null || request.Image.Length == 0)
                return BadRequest(ApiResponse<string>.Fail("Image is required"));

            var product = await _service.AddProductAsync(request);

            return Ok(ApiResponse<ProductDto>.SuccessResponse(
                product,
                "Product added successfully"
            ));
        }

        //  Get All Products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new ProductQueryParams
            {
                PageNumber = 1,
                PageSize = 50
            };

            var result = await _service.GetProductsAsync(query);

            return Ok(ApiResponse<PagedResult<ProductDto>>.SuccessResponse(
                result,
                "All products fetched successfully"
            ));
        }

        //  Get Products By Category
        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var query = new ProductQueryParams
            {
                CategoryId = categoryId,
                PageNumber = 1,
                PageSize = 50
            };

            var result = await _service.GetProductsAsync(query);

            return Ok(ApiResponse<PagedResult<ProductDto>>.SuccessResponse(
                result,
                "Products fetched by category successfully"
            ));
        }

        //  Get Product By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetProductByIdAsync(id);

            if (product == null)
                return NotFound(ApiResponse<string>.Fail("Product not found"));

            return Ok(ApiResponse<ProductDto>.SuccessResponse(
                product,
                "Product fetched successfully"
            ));
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductRequestDto request)
        {
            var updatedProduct = await _service.UpdateProductAsync(id, request);

            if (updatedProduct == null)
                return NotFound(ApiResponse<string>.Fail("Product not found"));

            return Ok(ApiResponse<ProductDto>.SuccessResponse(
                updatedProduct,
                "Product updated successfully"
            ));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteProductAsync(id);

            if (!deleted)
                return NotFound(ApiResponse<string>.Fail("Product not found"));

            return Ok(ApiResponse<string>.SuccessResponse(
                "Product deleted successfully",
                "Success"
            ));
        }
    }
}