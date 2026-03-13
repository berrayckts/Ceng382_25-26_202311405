using Microsoft.AspNetCore.Identity;

namespace Ceng382_25_26_202311405.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string? ProfilePhotoPath { get; set; }
}
