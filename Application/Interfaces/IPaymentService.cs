using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> PayAsync(string userId, int orderId);
        bool VerifyPayment(RazorpayVerifyDto model);
    }

}

