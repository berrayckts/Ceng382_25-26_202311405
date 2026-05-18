using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Northwind.Mvc.Models;
using Northwind.Mvc.Services;
using Northwind.Mvc.ViewModels;

namespace Northwind.Mvc.Controllers;

public class HomeController(CateringProjectContext db, ICartService carts, IOptions<CateringPlatformOptions> options) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var cfg = options.Value;
        var featuredCaterers = await db.Caterers
            .Include(c => c.MenuItems.Where(m => m.IsActive))
                .ThenInclude(m => m.Category)
            .Include(c => c.Orders)
                .ThenInclude(o => o.Reviews)
            .Where(c => c.IsApproved && c.MenuItems.Any(m => m.IsActive))
            .OrderBy(c => c.Name)
            .Take(3)
            .ToListAsync();

        var signatureMenus = await db.MenuItems
            .Include(m => m.Caterer)
            .Include(m => m.Category)
            .Where(m => m.IsActive)
            .OrderByDescending(m => m.Reviews.Count)
            .ThenBy(m => m.Name)
            .Take(3)
            .ToListAsync();

        return View(new HomeIndexViewModel
        {
            FeaturedCaterers = featuredCaterers,
            SignatureMenus = signatureMenus,
            Cart = carts.GetCart(HttpContext.Session),
            DefaultLatitude = cfg.DefaultLatitude,
            DefaultLongitude = cfg.DefaultLongitude
        });
    }

    [AllowAnonymous]
    public IActionResult Privacy() => View();

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
