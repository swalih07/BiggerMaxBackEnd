using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UpdateProductRequestDto
    {
        
            public string? Name { get; set; }
            public string? Description { get; set; }

            public decimal? Price { get; set; }      
            public int? Quantity { get; set; }       
            public int? CategoryId { get; set; }    

            public IFormFile? Image { get; set; }
        }
   }

