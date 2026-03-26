using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
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

    // ✅ 1️⃣ Paged + Filter + Search + Sort
    public async Task<PagedResult<ProductDto>>
        GetPagedResultAsync(ProductQueryParams query)
    {
        if (query.PageNumber <= 0)
            query.PageNumber = 1;

        if (query.PageSize <= 0 || query.PageSize > 100)
            query.PageSize = 10;

        var products = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .AsQueryable();

        // ✅ Filter by CategoryId
        if (query.CategoryId.HasValue)
        {
            products = products.Where(p =>
                p.CategoryId == query.CategoryId.Value);
        }
        // Optional: filter by category name
        else if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var categoryName = query.Category.ToLower();
            products = products.Where(p =>
                p.Category != null &&
                p.Category.Name.ToLower() == categoryName);
        }

        // ✅ Search (case-insensitive)
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            products = products.Where(p =>
                p.Name.ToLower().Contains(search));
        }

        // ✅ Sorting
        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            switch (query.SortBy.ToLower())
            {
                case "price":
                    products = query.Desc
                        ? products.OrderByDescending(p => p.Price)
                        : products.OrderBy(p => p.Price);
                    break;

                case "name":
                    products = query.Desc
                        ? products.OrderByDescending(p => p.Name)
                        : products.OrderBy(p => p.Name);
                    break;

                default:
                    products = products.OrderByDescending(p => p.Id);
                    break;
            }
        }
        else
        {
            products = products.OrderByDescending(p => p.Id);
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
                ImageUrl = p.ImageUrl,
                Quantity = p.Quantity,
                Category = p.Category != null
                    ? p.Category.Name
                    : string.Empty
            })
            .ToListAsync();

        return new PagedResult<ProductDto>
        {
            Items = items,
            TotalCount = totalCount
        };
    }

    // ✅ 2️⃣ Required by Interface
    public async Task<List<Product>>
        GetProductsByCategoryAsync(int categoryId)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .ToListAsync();
    }
    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity,
                Category = p.Category != null ? p.Category.Name : ""
            })
            .FirstOrDefaultAsync();

        return product;
    }
    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        var products = await _context.Products
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty
            })
            .ToListAsync();

        return products;
    }
}