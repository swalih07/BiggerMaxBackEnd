using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CheckoutResponseDto
    {
        public int OrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
