using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{

    public class PaymentResponseDto
    {
        public int OrderId { get; set; }
        public string RazorpayKey { get; set; } = string.Empty;
        public string RazorpayOrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}


