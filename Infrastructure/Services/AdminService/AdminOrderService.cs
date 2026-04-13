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

        // =========================
        // GET ALL ORDERS (NESTED)
        // =========================
        public async Task<List<OrderResponseDto>> GetAllOrderItemsAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return orders.Select(o => new OrderResponseDto
            {
                OrderId = o.Id,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                Date = o.CreatedAt,
                Items = o.OrderDetails.Select(od => new OrderItemDto
                {
                    ProductId = od.ProductId,
                    Name = od.Product.Name,
                    Price = od.Price,
                    Quantity = od.Quantity,
                    ImageUrl = od.Product.ImageUrl
                }).ToList()
            }).ToList();
        }

        // =========================
        // UPDATE STATUS
        // =========================
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return false;

            if (!Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
                throw new Exception("Invalid order status. Use: Pending, Processing, Shipped, Delivered, or Cancelled.");

            order.Status = parsedStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        // =========================
        // GET ORDER BY ID
        // =========================
        public async Task<List<OrderResponseDto>> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return new List<OrderResponseDto>();

            var dto = new OrderResponseDto
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                Date = order.CreatedAt,
                Items = order.OrderDetails.Select(od => new OrderItemDto
                {
                    ProductId = od.ProductId,
                    Name = od.Product.Name,
                    Price = od.Price,
                    Quantity = od.Quantity,
                    ImageUrl = od.Product.ImageUrl
                }).ToList()
            };

            return new List<OrderResponseDto> { dto };
        }
    }
}