using Northwind.Mvc.Models;

namespace Northwind.Mvc.Services;

public interface IEmailService
{
    Task SendOrderSummaryAsync(OrderEntity order, string? receiptEmail = null);
    Task SendTwoFactorCodeAsync(AppUser user, string code);
    Task SendVerificationCodeAsync(string toEmail, string code);
}
