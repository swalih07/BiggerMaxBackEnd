using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class ShippingAddressService : IShippingAddressService
    {
        private readonly AppDbContext _context;

        public ShippingAddressService(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // ADD ADDRESS
        // =========================
        public async Task AddAsync(string userId, AddShippingAddressDto dto)
        {
            int userIdInt = int.Parse(userId);   // 🔥 FIX

            var address = new ShippingAddress
            {
                UserId = userIdInt,   // ✅ int
                FullName = dto.FullName,
                Phone = dto.Phone,
                AddressLine = dto.AddressLine,
                City = dto.City,
                State = dto.State,
                Pincode = dto.Pincode
            };

            _context.ShippingAddresses.Add(address);
            await _context.SaveChangesAsync();
        }

        // =========================
        // GET ALL ADDRESSES
        // =========================
        public async Task<List<ShippingAddressDto>> GetAllAsync(string userId)
        {
            int userIdInt = int.Parse(userId);   // 🔥 FIX

            return await _context.ShippingAddresses
                .Where(x => x.UserId == userIdInt)   // ✅ int == int
                .Select(x => new ShippingAddressDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    AddressLine = x.AddressLine,
                    City = x.City,
                    State = x.State,
                    Pincode = x.Pincode
                })
                .ToListAsync();
        }

        // =========================
        // UPDATE ADDRESS
        // =========================
        public async Task UpdateAsync(string userId, UpdateShippingAddressDto dto)
        {
            int userIdInt = int.Parse(userId);   // 🔥 FIX

            var address = await _context.ShippingAddresses
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.Id &&
                    x.UserId == userIdInt);   // ✅ int == int

            if (address == null)
                throw new Exception("Address not found");

            address.FullName = dto.FullName;
            address.Phone = dto.Phone;
            address.AddressLine = dto.AddressLine;
            address.City = dto.City;
            address.State = dto.State;
            address.Pincode = dto.Pincode;

            await _context.SaveChangesAsync();
        }

        // =========================
        // DELETE ADDRESS
        // =========================
        public async Task DeleteAsync(string userId, int id)
        {
            int userIdInt = int.Parse(userId);   // 🔥 FIX

            var address = await _context.ShippingAddresses
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.UserId == userIdInt);   // ✅ int == int

            if (address == null)
                throw new Exception("Address not found");

            _context.ShippingAddresses.Remove(address);
            await _context.SaveChangesAsync();
        }
    }
}
