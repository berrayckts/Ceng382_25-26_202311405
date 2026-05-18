using System.ComponentModel.DataAnnotations;

namespace Northwind.Mvc.ViewModels;

public class RegisterViewModel
{
    [Required, StringLength(120)]
    public string FullName { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, StringLength(80, MinimumLength = 6), DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required]
    public string Role { get; set; } = "User";

    [StringLength(120)]
    public string? CatererName { get; set; }

    [StringLength(80)]
    public string? City { get; set; }
}

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = "";
}

public class VerifyEmailViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, StringLength(6, MinimumLength = 6)]
    [RegularExpression("\\d{6}", ErrorMessage = "Enter the 6-digit verification code.")]
    public string Code { get; set; } = "";
}

public class TwoFactorViewModel
{
    [Required]
    public int UserId { get; set; }

    [Required, StringLength(6, MinimumLength = 6)]
    public string Code { get; set; } = "";
}
