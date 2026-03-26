using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task <CheckoutResponseDto> CheckoutAsync(string userId, CheckoutDto dto);
        Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId);
        Task<bool> CancelOrderAsync(string userId, int orderId);
    }
}
