using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.AdminInterfaces;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly AppDbContext _context;

        public AdminOrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminAllOrderItemsDto>> GetAllOrderItemsAsync()
        {
            var result = await (
                from o in _context.Orders
                join u in _context.Users on o.UserId equals u.Id
                join od in _context.OrderDetails on o.Id equals od.OrderId
                join p in _context.Products on od.ProductId equals p.Id
                select new AdminAllOrderItemsDto
                {
                    OrderId = o.Id,
                    UserEmail = u.Email,
                    ProductName = p.Name,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),
                    CreatedAt = o.CreatedAt
                }
            ).ToListAsync();

            return result;
        }
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return false;

            if (!Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
                throw new Exception("Invalid order status");

            
            if (order.Status == OrderStatus.Delivered)
                throw new Exception("Delivered order cannot be modified");

            order.Status = parsedStatus;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<AdminAllOrderItemsDto>> GetOrderByIdAsync(int orderId)
        {
            var result = await (
                from o in _context.Orders
                join u in _context.Users on o.UserId equals u.Id
                join od in _context.OrderDetails on o.Id equals od.OrderId
                join p in _context.Products on od.ProductId equals p.Id
                where o.Id == orderId
                select new AdminAllOrderItemsDto
                {
                    OrderId = o.Id,
                    UserEmail = u.Email,
                    ProductName = p.Name,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),
                    CreatedAt = o.CreatedAt
                }
            ).ToListAsync();

            return result;
        }
    }
}