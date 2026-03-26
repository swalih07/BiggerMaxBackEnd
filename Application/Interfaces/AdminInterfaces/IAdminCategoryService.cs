using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.AdminInterfaces
{
    public interface IAdminCategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequestDto request);
        Task<bool> DeleteCategoryAsync(int id);
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
    }
}
