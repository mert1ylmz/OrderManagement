using OrderService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace OrderService.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    public EmailService(ILogger<EmailService> logger) => _logger = logger;

    public async Task SendOrderConfirmationEmail(string customerName, string orderNumber)
    {
        // 2 saniye gecikme simülasyonu
        await Task.Delay(2000);
        _logger.LogInformation($"[BACKGOUND JOB] {customerName} için {orderNumber} nolu sipariş maili gönderildi.");
    }
}