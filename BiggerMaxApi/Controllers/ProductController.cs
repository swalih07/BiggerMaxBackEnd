using Application.DTOs;
using Application.Interfaces;
using BiggerMaxApi.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiggerMaxApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductQueryParams query)
    {
        var result = await _productService.GetPagedResultAsync(query);

        return Ok(ApiResponse<PagedResult<ProductDto>>
            .SuccessResponse(result, "Products fetched successfully"));
    }

}
