using Application.DTOs;
using Application.Interfaces;
using BiggerMaxApi.Common;
using Microsoft.Extensions.Options;
using Razorpay.Api;

namespace Infrastructure.Payment
{
    public class RazorpayService : IPaymentGatewayService
    {
        private readonly RazorpaySettings _settings;

        public RazorpayService(IOptions<RazorpaySettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<RazorpayOrderDto> CreateRazorpayOrderAsync(decimal amount, int orderId)
        {
            try
            {
                // Initialize Razorpay Client with Key and Secret (matching appsettings.json)
                var client = new RazorpayClient(_settings.Key, _settings.Secret);

                // Prepare order options (Amount must be in Paise: ₹1 = 100 Paise)
                var options = new Dictionary<string, object>
                {
                    { "amount", (int)(amount * 100) },
                    { "currency", "INR" },
                    { "receipt", $"receipt_order_{orderId}" }
                };

                // Create Order in Razorpay
                var order = client.Order.Create(options);
                var razorpayOrderId = order["id"].ToString();

                return new RazorpayOrderDto
                {
                    OrderId = orderId,
                    RazorpayKey = _settings.Key,
                    RazorpayOrderId = razorpayOrderId,
                    Amount = amount
                };
            }
            catch (Exception ex)
            {
                // Log detailed error and throw it
                Console.WriteLine($"[Razorpay ERROR] {ex.Message}");
                throw new Exception($"Razorpay Order Creation Failed: {ex.Message}");
            }
        }
    }
}
