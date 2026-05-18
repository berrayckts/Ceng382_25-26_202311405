using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Services;

public class CateraUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<int>> roleManager,
    IOptions<IdentityOptions> options)
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole<int>>(userManager, roleManager, options)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        if (!string.IsNullOrWhiteSpace(user.FullName))
        {
            identity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));
        }
        if (user.CustomerProfileId.HasValue)
        {
            identity.AddClaim(new Claim("CustomerId", user.CustomerProfileId.Value.ToString()));
        }
        if (user.CatererProfileId.HasValue)
        {
            identity.AddClaim(new Claim("CatererId", user.CatererProfileId.Value.ToString()));
            identity.AddClaim(new Claim("OwnedRestaurantId", user.CatererProfileId.Value.ToString()));
        }

        return identity;
    }
}
