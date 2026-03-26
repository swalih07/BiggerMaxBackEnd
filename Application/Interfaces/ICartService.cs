using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(string userId, AddToCartDto dto);
        Task<CartResponseDto> GetCartAsync(string userId);
        Task<bool> UpdateCartAsync(string userId, UpdateCartDto dto);
        Task<bool> DeleteCartItemAsync(string userId, int productId);
    }
}
