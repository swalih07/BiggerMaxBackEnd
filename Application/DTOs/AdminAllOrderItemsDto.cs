using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AdminAllOrderItemsDto
    {
        public int OrderId { get; set; }
        public string UserEmail {  get; set; }=string.Empty;
        public string ProductName {  get; set; }=string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }=string.Empty;
        public DateTime CreatedAt {  get; set; }
    }
}
