using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ShippingAddress
    {
            public int Id { get; set; }
            public int UserId { get; set; } 

            public string FullName { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string AddressLine { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string State { get; set; } = string.Empty;
            public string Pincode { get; set; } = string.Empty;
        }

}
