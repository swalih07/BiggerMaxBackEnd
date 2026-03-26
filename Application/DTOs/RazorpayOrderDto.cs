using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RazorpayOrderDto
    {
        public int OrderId { get; set; }
        public string RazorpayKey { get; set; }
        public decimal Amount { get; set; }
    }
}
