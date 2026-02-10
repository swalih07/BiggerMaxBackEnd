using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public class ICartService
    {
        Task AddToCartAsync(string userId, AddToCartDto dto);
    }
}
