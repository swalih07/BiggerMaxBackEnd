using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.AdminInterfaces
{
    public interface IAdminUserService
    {
        Task<object> GetAllUsersAsync();
        Task<object?> GetUserWithOrdersAsync(int id);
        Task<bool> BlockUserAsync(int id, string currentAdminId);
        Task<bool> UnblockUserAsync(int id);
        Task<bool> ChangeUserRoleAsync(int id, ChangeUserRoleDto dto, string currentAdminId);
        Task<bool> DeleteUserAsync(int id, string currentAdminId);
        Task<List<ProductOrderedUsersDto>> GetUsersByProductIdAsync(int productId);
    }
}
