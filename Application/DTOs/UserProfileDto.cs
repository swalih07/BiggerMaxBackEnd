using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserProfileDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contact { get; set; }= string.Empty;
        public string ShippingAddress { get; set; }= string.Empty;
        public string Role { get; set; }

    }
}
