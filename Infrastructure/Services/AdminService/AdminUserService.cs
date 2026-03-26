using Microsoft.EntityFrameworkCore;
using Application.DTOs;
using Application.Interfaces.AdminInterfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.AdminService
{
    public class AdminUserService:IAdminUserService
    {
        private readonly AppDbContext _context;

        public AdminUserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Role,
                    u.IsActive,
                    u.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<object?> GetUserWithOrdersAsync(int id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Role,
                    u.IsActive,
                    u.CreatedAt,
                    Orders = _context.Orders
                        .Where(o => o.UserId == u.Id)
                        .Select(o => new
                        {
                            o.Id,
                            o.TotalAmount,
                            o.CreatedAt
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> BlockUserAsync(int id, string currentAdminId)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.Role == "Admin")
                return false;

            if (currentAdminId == user.Id.ToString())
                return false;

            user.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnblockUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            user.IsActive = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeUserRoleAsync(int id, ChangeUserRoleDto dto, string currentAdminId)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            if (dto.Role != "Admin" && dto.Role != "User")
                return false;

            if (currentAdminId == user.Id.ToString())
                return false;

            user.Role = dto.Role;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<ProductOrderedUsersDto>> GetUsersByProductIdAsync(int productId)
        {
            return await _context.OrderDetails
                .Where(od => od.ProductId == productId)
                .Include(od => od.Order)
                    .ThenInclude(o => o.User)
                .Select(od => new ProductOrderedUsersDto
                {
                    UserId = od.Order.UserId,          
                    Email = od.Order.User.Email,
                    OrderId = od.Order.Id,
                    OrderedAt = od.Order.CreatedAt,
                    Quantity = od.Quantity
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteUserAsync(int id, string currentAdminId)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.Role == "Admin")
                return false;

            if (currentAdminId == user.Id.ToString())
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
