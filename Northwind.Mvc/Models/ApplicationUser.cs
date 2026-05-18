using Microsoft.AspNetCore.Identity;

namespace Northwind.Mvc.Models;

public class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public bool IsEmailVerified { get; set; } = true;
    public string? EmailVerificationCode { get; set; }
    public DateTime? EmailVerificationCodeExpiresAt { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public DateTime? LastVerificationCodeSentAt { get; set; }
    public int? CatererProfileId { get; set; }
    public int? CustomerProfileId { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public CatererProfile? CatererProfile { get; set; }
    public CustomerProfile? CustomerProfile { get; set; }
}
