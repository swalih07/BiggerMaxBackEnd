using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProductOrderedUsersDto
    {
        public int UserId { get; set; } 
        public string Email { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public DateTime OrderedAt { get; set; }
        public int Quantity { get; set; }
    }
}
