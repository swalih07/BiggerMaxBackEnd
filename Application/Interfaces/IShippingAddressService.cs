using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IShippingAddressService
    {
        Task AddAsync(string userId, AddShippingAddressDto dto);
        Task<List<ShippingAddressDto>> GetAllAsync(string userId);
        Task UpdateAsync(string userId, UpdateShippingAddressDto dto);
        Task DeleteAsync(string userId, int id);
    }
}
