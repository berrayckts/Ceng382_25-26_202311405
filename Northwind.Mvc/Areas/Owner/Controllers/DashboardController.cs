using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Areas.Owner.Controllers;

public class DashboardController(CateringProjectContext db) : OwnerBaseController
{
    public async Task<IActionResult> Index()
    {
        var restaurantId = GetOwnerRestaurantId();
        var today = DateTime.UtcNow.Date;
        ViewBag.Restaurant = await db.Caterers.FindAsync(restaurantId);
        ViewBag.OrdersToday = await db.Orders.CountAsync(o => o.CatererId == restaurantId && o.CreatedUtc >= today);
        ViewBag.RevenueToday = await db.Orders.Where(o => o.CatererId == restaurantId && o.CreatedUtc >= today).SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
        ViewBag.PendingOrders = await db.Orders.CountAsync(o => o.CatererId == restaurantId && (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Preparing));
        ViewBag.MenuCount = await db.MenuItems.CountAsync(m => m.CatererId == restaurantId);
        ViewBag.PendingList = await db.Orders
            .Include(o => o.Customer)
            .Where(o => o.CatererId == restaurantId && (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Preparing))
            .OrderByDescending(o => o.CreatedUtc)
            .Take(8)
            .ToListAsync();
        return View();
    }
}
