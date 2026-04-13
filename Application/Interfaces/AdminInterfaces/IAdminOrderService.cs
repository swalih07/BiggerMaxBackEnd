using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.AdminInterfaces
{
    public interface IAdminOrderService
    {
        Task<List<OrderResponseDto>> GetAllOrderItemsAsync();
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        Task<List<OrderResponseDto>> GetOrderByIdAsync(int orderId);
    }
}
