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
        // PLACE FULL CART ORDER
        // =========================
        public async Task<OrderResponseDto> PlaceCartOrderAsync(string userId, PlaceOrderDto dto)
        {
            int userIdInt = int.Parse(userId);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userIdInt);

            if (cart == null || !cart.CartItems.Any())
                throw new Exception("Cart is empty");

            var shippingAddress = new ShippingAddress
            {
                UserId = userIdInt,
                FullName = dto.Name,
                Phone = dto.Phone,
                AddressLine = dto.Address,
                Pincode = dto.Pincode,
                State = dto.State,
                City = dto.State
            };
            _context.ShippingAddresses.Add(shippingAddress);
            await _context.SaveChangesAsync();

            var totalAmount = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity);

            var order = new Order
            {
                UserId = userIdInt,
                ShippingAddressId = shippingAddress.Id,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart.CartItems)
            {
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                });
            }

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();

            return new OrderResponseDto
            {
                OrderId = order.Id,
                TotalAmount = totalAmount,
                Status = order.Status.ToString(),
                Date = order.CreatedAt
            };
        }

        // =========================
        // GET USER ORDERS (FIXED)
        // =========================
        public async Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId)
        {
            int userIdInt = int.Parse(userId);

            // ✅ CRITICAL FIX: Include OrderDetails and Product to show images and items
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Where(o => o.UserId == userIdInt)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return orders.Select(o => new OrderResponseDto
            {
                OrderId = o.Id,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                Date = o.CreatedAt,
                // ✅ MAP ITEMS: This ensures the frontend gets the ImageUrl
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

            // Only allow cancellation if order is Pending, Paid, or Processing (before Shipping)
            if (order.Status == OrderStatus.Pending || 
                order.Status == OrderStatus.Paid || 
                order.Status == OrderStatus.Processing)
            {
                order.Status = OrderStatus.Cancelled;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
