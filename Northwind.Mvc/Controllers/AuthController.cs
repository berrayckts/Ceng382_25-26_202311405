using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Controllers;

public class AuthController : Controller
{
    [AllowAnonymous]
    public IActionResult AccessDenied(int? code)
    {
        ViewBag.StatusCode = code ?? 403;
        ViewBag.CurrentRole = User.IsInRole(AppRole.Admin)
            ? AppRole.Admin
            : User.IsInRole(AppRole.RestaurantOwner) || User.IsInRole(AppRole.LegacyCaterer)
                ? AppRole.RestaurantOwner
                : User.IsInRole(AppRole.Customer) || User.IsInRole(AppRole.LegacyUser)
                    ? AppRole.Customer
                    : "Guest";
        return View();
    }
}
