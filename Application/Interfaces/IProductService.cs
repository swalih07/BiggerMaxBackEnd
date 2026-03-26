using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetPagedResultAsync(ProductQueryParams query);
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<List<ProductDto>> GetAllProductsAsync();
    }
}
