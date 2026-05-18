namespace Northwind.Mvc.Models;

public class CateringPlatformOptions
{
    public decimal DefaultLatitude { get; set; } = 41.0082m;
    public decimal DefaultLongitude { get; set; } = 28.9784m;
    public double NearbyRadiusKm { get; set; } = 25;
    public string EmailFrom { get; set; } = "noreply@caterease.local";
    public string? SmtpHost { get; set; }
    public int SmtpPort { get; set; } = 587;
    public string? SmtpUser { get; set; }
    public string? SmtpPassword { get; set; }
    public bool SmtpEnableSsl { get; set; } = true;
    public int SmtpTimeoutMs { get; set; } = 3000;

    public bool HasSmtpConfiguration =>
        !string.IsNullOrWhiteSpace(SmtpHost)
        && !string.IsNullOrWhiteSpace(EmailFrom);
}
