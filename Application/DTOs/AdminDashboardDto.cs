using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int UniqueUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int TotalProductsPurchased { get; set; }

        public List<OrderTrendDto> RevenueTrend { get; set; } = new List<OrderTrendDto>();
        public List<TopProductDto> TopProducts { get; set; } = new List<TopProductDto>();
    }

    public class OrderTrendDto
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int TotalQuantity { get; set; }
    }
}
