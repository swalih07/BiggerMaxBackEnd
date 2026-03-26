using Application.DTOs;
using Application.Interfaces;
using BiggerMaxApi.Common;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;


namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly RazorpaySettings _razorpaySettings;

        public PaymentService(
     AppDbContext context,
     IPaymentGatewayService paymentGatewayService,
     IOptions<RazorpaySettings> razorpayOptions)
        {
            _context = context;
            _paymentGatewayService = paymentGatewayService;
            _razorpaySettings = razorpayOptions.Value;
        }

        public async Task<PaymentResponseDto> PayAsync(string userId, int orderId)
        {
            if (!int.TryParse(userId, out int userIdInt))
                throw new Exception("Invalid user id");

            var order = await _context.Orders
                .FirstOrDefaultAsync(o =>
                    o.Id == orderId &&
                    o.UserId == userIdInt);

            if (order == null)
                throw new Exception("Order not found");

            if (order.Status != OrderStatus.Pending)
                throw new Exception("Order already processed");

            // Optional: call gateway if needed
            await _paymentGatewayService
                .CreateRazorpayOrderAsync(order.TotalAmount, order.Id);

            return new PaymentResponseDto
            {
                OrderId = order.Id,
                RazorpayKey = _razorpaySettings.Key,
                Amount = order.TotalAmount
            };
        }


        public bool VerifyPayment(RazorpayVerifyDto model)
        {
            var secret = _razorpaySettings.KeySecret;

            string payload = model.RazorpayOrderId + "|" + model.RazorpayPaymentId;

            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(payloadBytes);
            var generatedSignature = BitConverter.ToString(hash)
                                    .Replace("-", "")
                                    .ToLower();

            return generatedSignature == model.RazorpaySignature;
        }

    }
}
