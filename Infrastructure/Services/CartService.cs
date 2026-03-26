using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // ADD TO CART
        // =========================
        public async Task AddToCartAsync(string userId, AddToCartDto dto)
        {
            int userIdInt = int.Parse(userId);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userIdInt);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userIdInt
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };

                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        // =========================
        // GET CART
        // =========================
        public async Task<CartResponseDto> GetCartAsync(string userId)
        {
            int userIdInt = int.Parse(userId);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userIdInt);

            if (cart == null || !cart.CartItems.Any())
            {
                return new CartResponseDto
                {
                    Items = new List<CartItemDto>(),
                    TotalAmount = 0
                };
            }

            var items = cart.CartItems.Select(ci => new CartItemDto
            {
                CartItemId = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                Quantity = ci.Quantity,
                Price = ci.Product.Price,
                ImageUrl = ci.Product.ImageUrl // ⭐ IMAGE URL ADD
            }).ToList();

            return new CartResponseDto
            {
                Items = items,
                TotalAmount = items.Sum(i => i.Price * i.Quantity)
            };
        }

        // =========================
        // UPDATE CART
        // =========================
        public async Task<bool> UpdateCartAsync(string userId, UpdateCartDto dto)
        {
            int userIdInt = int.Parse(userId);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userIdInt);

            if (cart == null)
                return false;

            var item = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == dto.ProductId);

            if (item == null)
                return false;

            if (dto.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = dto.Quantity;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // =========================
        // DELETE ITEM
        // =========================
        public async Task<bool> DeleteCartItemAsync(string userId, int productId)
        {
            int userIdInt = int.Parse(userId);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userIdInt);

            if (cart == null)
                return false;

            var item = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == productId);

            if (item == null)
                return false;

            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}