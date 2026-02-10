using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductDto>>
        GetPagedResultAsync(ProductQueryParams query)
    {
        var products = _context.Products
            .Include(p => p.Category) // IMPORTANT
            .AsQueryable();

        // 🔎 Filtering by Category Name
        if (!string.IsNullOrEmpty(query.Category))
        {
            products = products.Where(p =>
                p.Category.Name == query.Category);
        }

        // 🔎 Search
        if (!string.IsNullOrEmpty(query.Search))
        {
            products = products.Where(p =>
                p.Name.Contains(query.Search));
        }

        // 🔃 Sorting
        if (query.SortBy?.ToLower() == "price")
        {
            products = query.Desc
                ? products.OrderByDescending(p => p.Price)
                : products.OrderBy(p => p.Price);
        }

        var totalCount = await products.CountAsync();

        var items = await products
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Category = p.Category.Name   // FIXED
            })
            .ToListAsync();

        return new PagedResult<ProductDto>
        {
            Items = items,
            TotalCount = totalCount
        };
    }
}
