using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;
using Northwind.Mvc.Services;
using Northwind.Mvc.ViewModels;

namespace Northwind.Mvc.Controllers;

[Authorize]
public class OrdersController(CateringProjectContext db, IPdfService pdfs, ILogService logs) : Controller
{
    public async Task<IActionResult> Index(string? search, string? status, int page = 1)
    {
        var query = db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Caterer)
            .Include(o => o.Lines).ThenInclude(l => l.MenuItem)
            .Include(o => o.Reviews)
            .AsQueryable();
        if (IsCustomer())
        {
            var customerId = int.Parse(User.FindFirstValue("CustomerId")!);
            query = query.Where(o => o.CustomerId == customerId);
        }
        if (IsRestaurantOwner())
        {
            var catererId = int.Parse(User.FindFirstValue("CatererId")!);
            query = query.Where(o => o.CatererId == catererId);
        }
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(o => o.Status == status);
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(o => o.Customer!.DisplayName.Contains(search) || o.Caterer!.Name.Contains(search) || o.Id.ToString() == search);
        }

        const int pageSize = 10;
        var total = await query.CountAsync();
        var orders = await query.OrderByDescending(o => o.CreatedUtc).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        ViewBag.Status = status;
        return View(new PagedResult<OrderEntity> { Items = orders, Page = page, PageSize = pageSize, TotalItems = total, Search = search });
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await db.Orders.Include(o => o.Customer).Include(o => o.Caterer).Include(o => o.Lines).ThenInclude(l => l.MenuItem).Include(o => o.Reviews).FirstOrDefaultAsync(o => o.Id == id);
        return order is null || !CanSee(order) ? NotFound() : View(order);
    }

    [Authorize(Roles = AppRole.Admin + "," + AppRole.RestaurantOwner + "," + AppRole.LegacyCaterer)]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var order = await db.Orders.FindAsync(id);
        if (order is null || !OrderStatus.All.Contains(status) || !CanManageStatus(order))
        {
            return NotFound();
        }
        order.Status = status;
        order.CompletedUtc = status == OrderStatus.Completed ? DateTime.UtcNow : order.CompletedUtc;
        await db.SaveChangesAsync();
        await logs.InfoAsync("Order.StatusChanged", $"Order={id}; Status={status}", actorEmail: User.Identity?.Name);
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Receipt(int id)
    {
        var order = await db.Orders.FindAsync(id);
        if (order is null || !CanSee(order)) return NotFound();
        return File(await pdfs.CreateReceiptAsync(id), "application/pdf", $"vellora-receipt-{id}.pdf");
    }

    public async Task<IActionResult> Agreement(int id)
    {
        var order = await db.Orders.FindAsync(id);
        if (order is null || !CanSee(order)) return NotFound();
        return File(await pdfs.CreateAgreementAsync(id), "application/pdf", $"vellora-agreement-{id}.pdf");
    }

    private bool CanSee(OrderEntity order) =>
        User.IsInRole(AppRole.Admin)
        || (IsCustomer() && order.CustomerId == int.Parse(User.FindFirstValue("CustomerId")!))
        || (IsRestaurantOwner() && order.CatererId == int.Parse(User.FindFirstValue("OwnedRestaurantId") ?? User.FindFirstValue("CatererId")!));

    private bool CanManageStatus(OrderEntity order) =>
        User.IsInRole(AppRole.Admin)
        || (IsRestaurantOwner() && order.CatererId == int.Parse(User.FindFirstValue("OwnedRestaurantId") ?? User.FindFirstValue("CatererId")!));

    private bool IsCustomer() => User.IsInRole(AppRole.Customer) || User.IsInRole(AppRole.LegacyUser);

    private bool IsRestaurantOwner() => User.IsInRole(AppRole.RestaurantOwner) || User.IsInRole(AppRole.LegacyCaterer);
}
