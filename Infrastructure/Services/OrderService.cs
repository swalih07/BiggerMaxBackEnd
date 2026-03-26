using Microsoft.EntityFrameworkCore;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // CHECKOUT (Single Cart Item)
        // =========================
        public async Task<CheckoutResponseDto> CheckoutAsync(string userId, CheckoutDto dto)
        {
            int userIdInt = int.Parse(userId);

            var cartItem = await _context.CartItems
                .Include(ci => ci.Product)
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci =>
                    ci.Id == dto.CartItemId &&
                    ci.Cart.UserId == userIdInt);

            if (cartItem == null)
                throw new Exception("Cart item not found");

            var address = await _context.ShippingAddresses
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.ShippingAddressId &&
                    x.UserId == userIdInt);

            if (address == null)
                throw new Exception("Invalid shipping address");

            var totalAmount = cartItem.Product.Price * cartItem.Quantity;

            var order = new Order
            {
                UserId = userIdInt,
                ShippingAddressId = dto.ShippingAddressId,
                TotalAmount = totalAmount,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _context.OrderDetails.Add(new OrderDetail
            {
                OrderId = order.Id,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                Price = cartItem.Product.Price
            });

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return new CheckoutResponseDto
            {
                OrderId = order.Id,
                ProductName = cartItem.Product.Name,
                Quantity = cartItem.Quantity,
                TotalAmount = totalAmount,
                ImageUrl = cartItem.Product.ImageUrl
            };
        }

        // =========================
        // GET USER ORDERS
        // =========================
        public async Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId)
        {
            int userIdInt = int.Parse(userId);

            var orders = await _context.Orders
                .Where(o => o.UserId == userIdInt)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return orders.Select(o => new OrderResponseDto
            {
                OrderId = o.Id,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString()
            }).ToList();
        }

        // =========================
        // CANCEL ORDER
        // =========================
        public async Task<bool> CancelOrderAsync(string userId, int orderId)
        {
            int userIdInt = int.Parse(userId);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o =>
                    o.Id == orderId &&
                    o.UserId == userIdInt);

            if (order == null)
                return false;

            if (order.Status == OrderStatus.Shipped)
                return false;

            order.Status = OrderStatus.Cancelled;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
