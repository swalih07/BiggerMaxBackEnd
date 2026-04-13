using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PlaceOrderDto
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
