using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Controllers;

[Authorize(Roles = AppRole.CustomerOnly)]
public class UserDashboardController(CateringProjectContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var customerId = int.Parse(User.FindFirstValue("CustomerId")!);
        var customer = await db.Customers.FindAsync(customerId);
        ViewBag.Customer = customer;
        ViewBag.TotalSpending = await db.Orders.Where(o => o.CustomerId == customerId).SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
        ViewBag.OrderCount = await db.Orders.CountAsync(o => o.CustomerId == customerId);
        ViewBag.RecentOrders = await db.Orders.Include(o => o.Caterer).Where(o => o.CustomerId == customerId).OrderByDescending(o => o.CreatedUtc).Take(5).ToListAsync();
        ViewBag.NearbyCaterers = await db.Caterers.Include(c => c.MenuItems).Where(c => c.IsApproved).Take(6).ToListAsync();
        return View();
    }
}
