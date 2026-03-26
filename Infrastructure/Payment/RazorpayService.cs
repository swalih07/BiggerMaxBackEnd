using Application.DTOs;
using Application.Interfaces;

namespace Infrastructure.Payment
{
    public class RazorpayService : IPaymentGatewayService
    {
        public Task<RazorpayOrderDto> CreateRazorpayOrderAsync(decimal amount, int orderId)
        {
            return Task.FromResult(new RazorpayOrderDto
            {
                OrderId = orderId,
                RazorpayKey = "rzp_test_SEs0ccd0xAKxwx", // from config later
                Amount = amount
            });
        }
    }
}
