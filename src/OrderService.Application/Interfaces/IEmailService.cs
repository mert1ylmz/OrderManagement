namespace OrderService.Application.Interfaces;

public interface IEmailService
{
    Task SendOrderConfirmationEmail(string customerName, string orderNumber);
}