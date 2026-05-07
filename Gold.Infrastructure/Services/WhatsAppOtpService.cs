using System.Security.Cryptography;
using Gold.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Gold.Infrastructure.Services;

public class WhatsAppOtpService : IOtpService
{
    private readonly ILogger<WhatsAppOtpService> _logger;

    public WhatsAppOtpService(ILogger<WhatsAppOtpService> logger)
    {
        _logger = logger;
    }

    public string GenerateOtp(int length = 6)
    {
        var max = (int)Math.Pow(10, length);
        var n = RandomNumberGenerator.GetInt32(0, max);
        return n.ToString().PadLeft(length, '0');
    }

    public Task SendWhatsAppOtpAsync(string phoneNumber, string customerName, string orderCode, string otp, CancellationToken cancellationToken = default)
    {
        var message =
            $"Hello {customerName}, your gold piece (Order {orderCode}) is ready for pickup. " +
            $"Your verification code is: {otp}. Please share it with the branch staff to receive your item.";

        _logger.LogInformation("[WhatsApp OTP] To: {Phone} | Order: {Order} | OTP: {Otp} | Message: {Message}",
            phoneNumber, orderCode, otp, message);

        return Task.CompletedTask;
    }
}
