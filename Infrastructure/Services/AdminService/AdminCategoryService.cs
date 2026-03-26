using Microsoft.EntityFrameworkCore;
using Application.DTOs;
using Application.Interfaces.AdminInterfaces;
using Domain.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infrastructure.Services.AdminService
    {
        public class AdminCategoryService : IAdminCategoryService
        {
            private readonly AppDbContext _context;

            public AdminCategoryService(AppDbContext context)
            {
                _context = context;
            }

            public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequestDto request)
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    throw new Exception("Category name is required");

                var exists = await _context.Categories
                    .AnyAsync(c => c.Name.ToLower() == request.Name.ToLower());

                if (exists)
                    throw new Exception("Category already exists");

                var category = new Category
                {
                    Name = request.Name.Trim()
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name
                };
            }
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return false;

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Category name cannot be empty");

            category.Name = dto.Name;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return false;

            
            var hasProducts = await _context.Products
                .AnyAsync(p => p.CategoryId == id);

            if (hasProducts)
                throw new Exception("Cannot delete category. Products exist under this category.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }
    }
    }

