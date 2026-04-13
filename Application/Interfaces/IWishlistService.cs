using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IWishlistService
    {
        Task ToggleWishlistAsync(string userId, AddToWishlistDto dto);
        Task<List<WishlistDto>> GetWishlistAsync(string userId);
        Task RemoveFromWishlistAsync(string userId, int productId);
        Task ClearWishlistAsync(string userId);
    }
}
