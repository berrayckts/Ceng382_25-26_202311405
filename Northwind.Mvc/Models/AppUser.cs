namespace Northwind.Mvc.Models;

public class AppUser
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Role { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsTwoFactorEnabled { get; set; }
    public string? TwoFactorCode { get; set; }
    public DateTime? TwoFactorExpiresUtc { get; set; }
    public int? CatererProfileId { get; set; }
    public int? CustomerProfileId { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public CatererProfile? CatererProfile { get; set; }
    public CustomerProfile? CustomerProfile { get; set; }
}
