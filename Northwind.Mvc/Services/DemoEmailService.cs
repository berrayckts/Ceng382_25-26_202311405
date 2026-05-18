using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Services;

public class DemoEmailService(
    CateringProjectContext db,
    ILogService logs,
    IOptions<CateringPlatformOptions> options,
    IPdfService pdfs) : IEmailService
{
    public async Task SendOrderSummaryAsync(OrderEntity order, string? receiptEmail = null)
    {
        var loaded = await db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Caterer)
            .Include(o => o.Lines).ThenInclude(l => l.MenuItem)
            .FirstAsync(o => o.Id == order.Id);

        var cfg = options.Value;
        var toUser = string.IsNullOrWhiteSpace(receiptEmail) ? loaded.Customer?.Email : receiptEmail.Trim();
        var toCaterer = loaded.Caterer?.Email;
        var subject = $"{ProjectBranding.Name} receipt and agreement for order #{loaded.Id}";
        var body = BuildOrderBody(loaded);

        if (string.IsNullOrWhiteSpace(cfg.SmtpHost))
        {
            var context = $"DemoMode=True; From={cfg.EmailFrom}; ToReceipt={toUser}; ToCaterer={toCaterer}; Order={loaded.Id}; Total={loaded.TotalAmount:C}";
            await logs.InfoAsync("Email.OrderSummary", context);
            return;
        }

        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(cfg.EmailFrom),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            bool IsValidRealEmail(string? email) =>
                !string.IsNullOrWhiteSpace(email) && email.Contains('@') &&
                !email.EndsWith(".local", StringComparison.OrdinalIgnoreCase) && 
                !email.EndsWith("test.com", StringComparison.OrdinalIgnoreCase);

            var validUserEmail = IsValidRealEmail(toUser) ? toUser : null;
            var validCatererEmail = IsValidRealEmail(toCaterer) ? toCaterer : null;

            if (validUserEmail == null && validCatererEmail == null)
            {
                // If both are demo emails, send to system email to prevent crash and prove it works
                message.To.Add(cfg.EmailFrom);
            }
            else
            {
                if (validUserEmail != null)
                {
                    message.To.Add(validUserEmail);
                }
                else if (validCatererEmail != null)
                {
                    message.To.Add(validCatererEmail);
                    validCatererEmail = null;
                }

                if (validCatererEmail != null && !string.Equals(validCatererEmail, validUserEmail, StringComparison.OrdinalIgnoreCase))
                {
                    message.CC.Add(validCatererEmail);
                }
            }

            var receiptStream = new MemoryStream(await pdfs.CreateReceiptAsync(loaded.Id));
            var agreementStream = new MemoryStream(await pdfs.CreateAgreementAsync(loaded.Id));
            message.Attachments.Add(new Attachment(receiptStream, $"vellora-receipt-{loaded.Id}.pdf", "application/pdf"));
            message.Attachments.Add(new Attachment(agreementStream, $"vellora-agreement-{loaded.Id}.pdf", "application/pdf"));

            using var smtp = new SmtpClient(cfg.SmtpHost, cfg.SmtpPort)
            {
                EnableSsl = cfg.SmtpEnableSsl,
                Timeout = Math.Clamp(cfg.SmtpTimeoutMs, 1000, 15000)
            };
            if (!string.IsNullOrWhiteSpace(cfg.SmtpUser))
            {
                smtp.Credentials = new NetworkCredential(cfg.SmtpUser, cfg.SmtpPassword);
            }

            await smtp.SendMailAsync(message);
            await logs.InfoAsync("Email.OrderSummary", $"Sent=True; ToReceipt={toUser}; ToCaterer={toCaterer}; Order={loaded.Id}");
        }
        catch (Exception ex)
        {
            await logs.ErrorAsync("Email.OrderSummaryFailed", ex, $"ToReceipt={toUser}; ToCaterer={toCaterer}; Order={loaded.Id}");
            throw;
        }
    }

    public Task SendTwoFactorCodeAsync(AppUser user, string code) =>
        logs.InfoAsync("Email.TwoFactor", $"To={user.Email}; DemoCode={code}", user.Id, user.Email);

    public async Task SendVerificationCodeAsync(string toEmail, string code)
    {
        var cfg = options.Value;
        var subject = "Your verification code";
        var body = new StringBuilder()
            .AppendLine($"Your {ProjectBranding.Name} verification code is {code}.")
            .AppendLine()
            .AppendLine("This code is valid for 10 minutes.")
            .AppendLine("If you did not create an account, you can ignore this email.")
            .ToString();

        if (!cfg.HasSmtpConfiguration || !IsValidRealEmail(toEmail))
        {
            await logs.InfoAsync("Email.VerificationCode", $"DemoMode=True; To={toEmail}; Code={code}", actorEmail: toEmail);
            return;
        }

        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(cfg.EmailFrom),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            message.To.Add(toEmail.Trim());

            using var smtp = new SmtpClient(cfg.SmtpHost, cfg.SmtpPort)
            {
                EnableSsl = cfg.SmtpEnableSsl,
                Timeout = Math.Clamp(cfg.SmtpTimeoutMs, 1000, 15000)
            };
            if (!string.IsNullOrWhiteSpace(cfg.SmtpUser))
            {
                smtp.Credentials = new NetworkCredential(cfg.SmtpUser, cfg.SmtpPassword);
            }

            await smtp.SendMailAsync(message);
            await logs.InfoAsync("Email.VerificationCode", $"Sent=True; To={toEmail}", actorEmail: toEmail);
        }
        catch (Exception ex)
        {
            await logs.ErrorAsync("Email.VerificationCodeFailed", ex, $"To={toEmail}");
            throw;
        }
    }

    private static bool IsValidRealEmail(string? email) =>
        !string.IsNullOrWhiteSpace(email) && email.Contains('@') &&
        !email.EndsWith(".local", StringComparison.OrdinalIgnoreCase) &&
        !email.EndsWith("test.com", StringComparison.OrdinalIgnoreCase);

    private static string BuildOrderBody(OrderEntity order)
    {
        var body = new StringBuilder();
        body.AppendLine($"Order #{order.Id}");
        body.AppendLine($"Customer: {order.Customer?.DisplayName}");
        body.AppendLine($"Caterer: {order.Caterer?.Name}");
        body.AppendLine($"Status: {order.Status}");
        body.AppendLine($"Total: {order.TotalAmount:C}");
        body.AppendLine();
        body.AppendLine("Items:");
        foreach (var line in order.Lines)
        {
            body.AppendLine($"- {line.Quantity} guests x {line.MenuItem?.Name} ({line.UnitPrice:C} per person) {line.CustomizationSummary}");
        }
        body.AppendLine();
        body.AppendLine("Receipt and agreement PDFs are attached when SMTP is configured.");
        return body.ToString();
    }
}
