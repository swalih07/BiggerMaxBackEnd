using Application.DTOs;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> PayAsync(string userId, int orderId);
        Task<bool> VerifyAndConfirmPaymentAsync(RazorpayVerifyDto model);
    }
}
