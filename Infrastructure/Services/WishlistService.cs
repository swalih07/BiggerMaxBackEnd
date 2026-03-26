using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly AppDbContext _context;

        public WishlistService(AppDbContext context)
        {
            _context = context;
        }

        //public async Task AddToWishlistAsync(string userId, AddToWishlistDto dto)
        //{
        //    if (string.IsNullOrEmpty(userId))
        //        throw new ArgumentException("Invalid user");

        //    // ✅ Check product exists
        //    var productExists = await _context.Products
        //        .AnyAsync(p => p.Id == dto.ProductId);

        //    if (!productExists)
        //        throw new Exception("Product not found");

        //    var wishlist = await _context.Wishlists
        //        .Include(w => w.WishlistItems)
        //        .FirstOrDefaultAsync(w => w.UserId == userId);

        //    if (wishlist == null)
        //    {
        //        wishlist = new Wishlist
        //        {
        //            UserId = userId,
        //            WishlistItems = new List<WishlistItem>()
        //        };

        //        _context.Wishlists.Add(wishlist);
        //    }

        //    var exists = wishlist.WishlistItems
        //        .Any(wi => wi.ProductId == dto.ProductId);

        //    if (exists)
        //        return; // Already added (silent ignore)

        //    wishlist.WishlistItems.Add(new WishlistItem
        //    {
        //        ProductId = dto.ProductId,
        //        Wishlist = wishlist
        //    });

        //    await _context.SaveChangesAsync();
        //}
        public async Task ToggleWishlistAsync(string userId, AddToWishlistDto dto)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Invalid user");

            var productExists = await _context.Products
                .AnyAsync(p => p.Id == dto.ProductId);

            if (!productExists)
                throw new Exception("Product not found");

            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId,
                    WishlistItems = new List<WishlistItem>()
                };

                _context.Wishlists.Add(wishlist);
            }

            var existingItem = wishlist.WishlistItems
                .FirstOrDefault(wi => wi.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                // 🔥 REMOVE (toggle off)
                _context.WishlistItems.Remove(existingItem);
            }
            else
            {
                // 🔥 ADD (toggle on)
                wishlist.WishlistItems.Add(new WishlistItem
                {
                    ProductId = dto.ProductId,
                    Wishlist = wishlist
                });
            }

            await _context.SaveChangesAsync();
        }


        public async Task<List<WishlistDto>> GetWishlistAsync(string userId)
        {
            var wishlist = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.WishlistItems)
                    .ThenInclude(wi => wi.Product)
                .FirstOrDefaultAsync();

            if (wishlist == null)
                return new List<WishlistDto>();

            return wishlist.WishlistItems
                .Select(wi => new WishlistDto
                {
                    WishlistId = wishlist.Id,
                    ProductId = wi.ProductId,
                    ProductName = wi.Product.Name,
                    Price = wi.Product.Price,
                    //ImageUrl = wi.Product.ImageUrl
                })
                .ToList();
        }
        //public async Task RemoveFromWishlistAsync(string userId,int productId)
        //{
        //    var wishlist = await _context.Wishlists
        //        .Include(w => w.WishlistItems)
        //        .FirstOrDefaultAsync(w => w.UserId == userId);

        //    if (wishlist == null)
        //        throw new Exception("Wishlist not Found");

        //    var item = wishlist.WishlistItems
        //        .FirstOrDefault(wi => wi.ProductId == productId);
        //    if (item == null)
        //        throw new Exception("Product not in Wishlist");

        //    _context.Remove(item);
        //    await _context.SaveChangesAsync();
        //}
        public async Task ClearWishlistAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Invalid user");

            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
                return; // nothing to clear

            // Remove all wishlist items
            _context.WishlistItems.RemoveRange(wishlist.WishlistItems);

            // Optional: also remove wishlist row itself
            _context.Wishlists.Remove(wishlist);

            await _context.SaveChangesAsync();
        }

    }
}
