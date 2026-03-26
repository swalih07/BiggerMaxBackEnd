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
        Task<List<AdminAllOrderItemsDto>> GetAllOrderItemsAsync();
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        Task<List<AdminAllOrderItemsDto>> GetOrderByIdAsync(int orderId);
    }
}
