using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IPaymentGatewayService
    {
        Task<RazorpayOrderDto> CreateRazorpayOrderAsync(decimal amount, int orderId);


    }
}
