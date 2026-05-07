namespace Gold.Application.Interfaces;

public interface IOtpService
{
    string GenerateOtp(int length = 6);
    Task SendWhatsAppOtpAsync(string phoneNumber, string customerName, string orderCode, string otp, CancellationToken cancellationToken = default);
}
