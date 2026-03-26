using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CheckoutDto
    {
        public int CartItemId { get; set; }
        public int ShippingAddressId { get; set; }
    }
}
