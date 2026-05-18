using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;
using Northwind.Mvc.Services;
using Northwind.Mvc.ViewModels;

namespace Northwind.Mvc.Controllers;

[Authorize(Roles = AppRole.Admin)]
public class AdminController(CateringProjectContext db, ILogService logs, IConfiguration configuration) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Users = await db.AppUsers.CountAsync();
        ViewBag.Caterers = await db.Caterers.CountAsync();
        ViewBag.ApprovedCaterers = await db.Caterers.CountAsync(c => c.IsApproved);
        ViewBag.PendingCaterers = await db.Caterers.CountAsync(c => !c.IsApproved);
        ViewBag.MenuItems = await db.MenuItems.CountAsync(m => m.IsActive);
        ViewBag.Orders = await db.Orders.CountAsync();
        ViewBag.OpenOrders = await db.Orders.CountAsync(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled);
        ViewBag.Revenue = await db.Orders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
        ViewBag.RecentLogs = await db.SystemLogs.OrderByDescending(l => l.CreatedUtc).Take(6).ToListAsync();
        ViewBag.RecentOrders = await db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Caterer)
            .OrderByDescending(o => o.CreatedUtc)
            .Take(6)
            .ToListAsync();
        return View();
    }

    public async Task<IActionResult> LinkRestaurants()
    {
        var cfg = configuration.GetSection("CateringPlatform").Get<CateringPlatformOptions>() ?? new CateringPlatformOptions();
        return View(new AdminLinkRestaurantsViewModel
        {
            DefaultLatitude = cfg.DefaultLatitude,
            DefaultLongitude = cfg.DefaultLongitude,
            Caterers = await db.Caterers.OrderBy(c => c.Name).ToListAsync()
        });
    }

    public async Task<IActionResult> Users(string? search, string? role, int page = 1)
    {
        var query = db.AppUsers.Include(u => u.CatererProfile).Include(u => u.CustomerProfile).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
        if (!string.IsNullOrWhiteSpace(role)) query = query.Where(u => u.Role == role);
        const int pageSize = 10;
        var total = await query.CountAsync();
        var items = await query.OrderBy(u => u.Role).ThenBy(u => u.FullName).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        ViewBag.Role = role;
        return View(new PagedResult<AppUser> { Items = items, Page = page, PageSize = pageSize, TotalItems = total, Search = search });
    }

    public async Task<IActionResult> Caterers(string? search, string? city, string? approval, int page = 1)
    {
        var query = db.Caterers.Include(c => c.MenuItems).Include(c => c.Orders).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(c => c.Name.Contains(search) || c.City!.Contains(search) || c.District!.Contains(search));
        if (!string.IsNullOrWhiteSpace(city)) query = query.Where(c => c.City == city);
        if (approval == "approved") query = query.Where(c => c.IsApproved);
        if (approval == "pending") query = query.Where(c => !c.IsApproved);
        const int pageSize = 10;
        var total = await query.CountAsync();
        var items = await query.OrderBy(c => c.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        ViewBag.City = city;
        ViewBag.Approval = approval;
        ViewBag.Cities = await db.Caterers
            .Where(c => c.City != null && c.City != "")
            .Select(c => c.City!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
        return View(new PagedResult<CatererProfile> { Items = items, Page = page, PageSize = pageSize, TotalItems = total, Search = search });
    }

    public async Task<IActionResult> Orders(string? search, string? status, int page = 1)
    {
        var query = db.Orders.Include(o => o.Customer).Include(o => o.Caterer).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(o => o.Customer!.DisplayName.Contains(search) || o.Caterer!.Name.Contains(search) || o.Id.ToString() == search);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(o => o.Status == status);
        const int pageSize = 10;
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(o => o.CreatedUtc).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        ViewBag.Status = status;
        return View(new PagedResult<OrderEntity> { Items = items, Page = page, PageSize = pageSize, TotalItems = total, Search = search });
    }

    public async Task<IActionResult> Logs(string? search, string? level, int page = 1)
    {
        var query = db.SystemLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(l => l.EventName.Contains(search) || l.ContextData!.Contains(search) || l.ActorEmail!.Contains(search));
        if (!string.IsNullOrWhiteSpace(level)) query = query.Where(l => l.Level == level);
        const int pageSize = 15;
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(l => l.CreatedUtc).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        ViewBag.Level = level;
        return View(new PagedResult<SystemLogEntry> { Items = items, Page = page, PageSize = pageSize, TotalItems = total, Search = search });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleUser(int id)
    {
        var user = await db.AppUsers.FindAsync(id);
        if (user is null) return NotFound();
        user.IsActive = !user.IsActive;
        await db.SaveChangesAsync();
        await logs.InfoAsync("Admin.UserToggled", $"User={id}; Active={user.IsActive}", actorEmail: User.Identity?.Name);
        return RedirectToAction(nameof(Users));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCatererApproval(int id, string? search, string? city, string? approval, int page = 1)
    {
        var caterer = await db.Caterers.FindAsync(id);
        if (caterer is null) return NotFound();

        caterer.IsApproved = !caterer.IsApproved;
        await db.SaveChangesAsync();
        await logs.InfoAsync("Admin.CatererApprovalToggled", $"Caterer={id}; Approved={caterer.IsApproved}", actorEmail: User.Identity?.Name);
        TempData["Success"] = caterer.IsApproved ? "Restaurant approved." : "Restaurant moved to pending.";
        return RedirectToAction(nameof(Caterers), new { search, city, approval, page });
    }
}
