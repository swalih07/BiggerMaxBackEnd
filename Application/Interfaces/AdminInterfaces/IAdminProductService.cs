using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.AdminInterfaces
{
    public interface IAdminProductService
    {
        //Task<ProductDto> AddProductAsync(CreateProductRequest request);
        Task<ProductDto> AddProductAsync(CreateProductRequest request);

        Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParams query);

        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductRequestDto request);
        Task<bool> DeleteProductAsync(int id);

    }
}
