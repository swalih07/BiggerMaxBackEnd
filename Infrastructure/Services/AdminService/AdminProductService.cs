using Application.DTOs;
using Application.Interfaces.AdminInterfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.AdminService
{
    public class AdminProductService : IAdminProductService
    {
        private readonly AppDbContext _context;
        private readonly CloudinaryService _cloudinary;

        public AdminProductService(AppDbContext context, CloudinaryService cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // Add Product
        public async Task<ProductDto> AddProductAsync(CreateProductRequest request)
        {
            if (request.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Id == request.CategoryId.Value);

                if (!categoryExists)
                    throw new Exception("Invalid CategoryId");
            }

            var imageUrl = await _cloudinary.UploadImageAsync(request.Image!);

            var product = new Product
            {
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Price = request.Price,
                Quantity = request.Quantity,
                CategoryId = request.CategoryId,
                ImageUrl = imageUrl
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            string categoryName = "";

            if (product.CategoryId.HasValue)
            {
                categoryName = await _context.Categories
                    .Where(c => c.Id == product.CategoryId.Value)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync() ?? "";
            }

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                ImageUrl = product.ImageUrl,
                Category = categoryName
            };
        }

        //  Admin Product Listing (Paged + Filter + Search + Sort)
        public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParams query)
        {
            if (query.PageNumber <= 0)
                query.PageNumber = 1;

            if (query.PageSize <= 0 || query.PageSize > 100)
                query.PageSize = 10;

            var products = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .AsQueryable();

            if (query.CategoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == query.CategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.ToLower();
                products = products.Where(p => p.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                switch (query.SortBy.ToLower())
                {
                    case "name":
                        products = query.Desc
                            ? products.OrderByDescending(p => p.Name)
                            : products.OrderBy(p => p.Name);
                        break;

                    case "price":
                        products = query.Desc
                            ? products.OrderByDescending(p => p.Price)
                            : products.OrderBy(p => p.Price);
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
                    Category = p.Category != null ? p.Category.Name : ""
                })
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        //  Get Single Product
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.Id == id)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    Quantity = p.Quantity,
                    Category = p.Category != null ? p.Category.Name : ""
                })
                .FirstOrDefaultAsync();
        }
        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductRequestDto request)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return null;

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(request.Name))
                product.Name = request.Name.Trim();

            if (!string.IsNullOrWhiteSpace(request.Description))
                product.Description = request.Description.Trim();

            if (request.Price.HasValue)
                product.Price = request.Price.Value;

            if (request.Quantity.HasValue)
                product.Quantity = request.Quantity.Value;

            if (request.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Id == request.CategoryId.Value);

                if (!categoryExists)
                    throw new Exception("Invalid CategoryId");

                product.CategoryId = request.CategoryId.Value;
            }

            if (request.Image != null)
            {
                var imageUrl = await _cloudinary.UploadImageAsync(request.Image);
                product.ImageUrl = imageUrl;
            }

            await _context.SaveChangesAsync();

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                ImageUrl = product.ImageUrl,
                Category = product.Category?.Name ?? ""
            };
        }
        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}