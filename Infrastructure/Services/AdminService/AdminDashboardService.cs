using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.AdminInterfaces;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly AppDbContext _context;

        public AdminDashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardDto> GetDashboardDataAsync()
        {
            // 1. Basic Counts
            var totalUsers = await _context.Users.CountAsync();
            var uniqueUsers = await _context.Orders
                .Select(o => o.UserId)
                .Distinct()
                .CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();

            var totalRevenue = await _context.Orders
                .Select(o => (decimal?)o.TotalAmount)
                .SumAsync() ?? 0;

            var pendingOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Pending)
                .CountAsync();

            var completedOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .CountAsync();

            var totalProductsPurchased = await _context.OrderDetails
                .Select(od => (int?)od.Quantity)
                .SumAsync() ?? 0;

            // 2. Revenue Trend (Last 30 Days)
            var last30Days = DateTime.UtcNow.Date.AddDays(-30);
            
            // Get raw order data from the last 30 days
            var ordersInPeriod = await _context.Orders
                .Where(o => o.CreatedAt >= last30Days)
                .Select(o => new { o.CreatedAt, o.TotalAmount })
                .ToListAsync();

            // Group and format in memory to avoid SQL translation issues
            var trendData = ordersInPeriod
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new OrderTrendDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(t => t.Date)
                .ToList();

            // 3. Top Products
            // Use grouping on the numeric ProductId first for cleaner SQL,
            // then bring it to memory for property mapping.
            var productSalesRaw = await _context.OrderDetails
                .GroupBy(od => od.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalRevenue = g.Sum(od => od.Price * od.Quantity),
                    TotalQuantity = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(g => g.TotalRevenue)
                .Take(6)
                .ToListAsync();

            // Load extra product info in memory
            var topProducts = new List<TopProductDto>();
            foreach (var ps in productSalesRaw)
            {
                var product = await _context.Products.FindAsync(ps.ProductId);
                topProducts.Add(new TopProductDto
                {
                    ProductId = ps.ProductId,
                    Name = product?.Name ?? "Unknown Product",
                    ImageUrl = product?.ImageUrl ?? string.Empty,
                    TotalRevenue = ps.TotalRevenue,
                    TotalQuantity = ps.TotalQuantity
                });
            }

            return new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                UniqueUsers = uniqueUsers,
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders,
                TotalProductsPurchased = totalProductsPurchased,
                RevenueTrend = trendData,
                TopProducts = topProducts
            };
        }
    }
}