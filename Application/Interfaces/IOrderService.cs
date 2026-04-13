using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<CheckoutResponseDto> CheckoutAsync(string userId, CheckoutDto dto);
        Task<OrderResponseDto> PlaceCartOrderAsync(string userId, PlaceOrderDto dto);
        Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId);
        Task<bool> CancelOrderAsync(string userId, int orderId);
    }
}
