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
                throw new Exception("Order is no longer in pending status");

            // Call gateway to create Razorpay Order
            var razorpayOrder = await _paymentGatewayService
                .CreateRazorpayOrderAsync(order.TotalAmount, order.Id);

            // ✅ Save RazorpayOrderId in local database to track it
            order.RazorpayOrderId = razorpayOrder.RazorpayOrderId;
            await _context.SaveChangesAsync();

            return new PaymentResponseDto
            {
                OrderId = order.Id,
                RazorpayKey = razorpayOrder.RazorpayKey,
                RazorpayOrderId = razorpayOrder.RazorpayOrderId,
                Amount = order.TotalAmount
            };
        }

        public async Task<bool> VerifyAndConfirmPaymentAsync(RazorpayVerifyDto model)
        {
            // 1. Verify Signature
            var secret = _razorpaySettings.Secret; // ✅ UPDATED FROM 'KeySecret' to 'Secret'
            string payload = model.RazorpayOrderId + "|" + model.RazorpayPaymentId;

            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(payloadBytes);
            var generatedSignature = BitConverter.ToString(hash)
                                    .Replace("-", "")
                                    .ToLower();

            bool isValid = generatedSignature == model.RazorpaySignature;
            if (!isValid) return false;

            // 2. Find and update the order
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.RazorpayOrderId == model.RazorpayOrderId);

            if (order == null) return false;

            order.Status = OrderStatus.Paid;
            order.RazorpayPaymentId = model.RazorpayPaymentId;
            
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
