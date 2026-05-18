using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Areas.Admin.Controllers;

public class DashboardController(CateringProjectContext db) : AdminBaseController
{
    public async Task<IActionResult> Index()
    {
        var today = DateTime.UtcNow.Date;
        ViewBag.OrdersToday = await db.Orders.CountAsync(o => o.CreatedUtc >= today);
        ViewBag.Revenue = await db.Orders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
        ViewBag.ActiveRestaurants = await db.Caterers.CountAsync(c => c.IsApproved);
        ViewBag.PendingOwners = await db.Caterers.CountAsync(c => !c.IsApproved);
        ViewBag.Users = await db.AppUsers.CountAsync();
        ViewBag.MenuItems = await db.MenuItems.CountAsync(m => m.IsActive);
        ViewBag.RecentOrders = await db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Caterer)
            .OrderByDescending(o => o.CreatedUtc)
            .Take(10)
            .ToListAsync();
        ViewBag.PendingRestaurants = await db.Caterers
            .Where(c => !c.IsApproved)
            .OrderBy(c => c.Name)
            .Take(6)
            .ToListAsync();
        return View();
    }
}
