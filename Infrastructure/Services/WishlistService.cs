using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class WishlistService:IWishlistService
    {
        private readonly AppDbContext _context;

        public WishlistService(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddToWishlistAsync(string userId,AddToWishlistDto dto)
        {
            var wishlist = await _context.Wishlists.Include(w => w.WishlistItems).FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId,
                    WishlistItems = new List<WishlistItem>()
                };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();

            }
            var exists = wishlist.WishlistItems.Any(wi => wi.ProductId == dto.ProductId);

            if (!exists)
            {
                wishlist.WishlistItems.Add(new WishlistItem
                {
                    ProductId = dto.ProductId
                });
                await _context.SaveChangesAsync();
            }
        }
    }
}
