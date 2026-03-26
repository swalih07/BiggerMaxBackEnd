using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class CartResponseDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }

    public class CartItemDto
    {
        public int CartItemId { get; set; }   // IMPORTANT

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty; // ADD THIS
    }

}
