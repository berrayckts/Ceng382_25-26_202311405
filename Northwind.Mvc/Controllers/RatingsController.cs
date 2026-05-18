using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;
using Northwind.Mvc.Services;
using Northwind.Mvc.ViewModels;

namespace Northwind.Mvc.Controllers;

[Authorize(Roles = AppRole.CustomerOnly)]
public class RatingsController(CateringProjectContext db, ILogService logs) : Controller
{
    public async Task<IActionResult> Create(int orderId, int menuItemId)
    {
        var customerId = int.Parse(User.FindFirstValue("CustomerId")!);
        var order = await db.Orders
            .Include(o => o.Caterer)
            .Include(o => o.Lines).ThenInclude(l => l.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId && OrderStatus.Rateable.Contains(o.Status));
        var line = order?.Lines.FirstOrDefault(l => l.MenuItemId == menuItemId);
        return line is null ? NotFound() : View(new RatingViewModel
        {
            OrderId = orderId,
            MenuItemId = menuItemId,
            MenuItemName = line.MenuItem?.Name ?? "",
            CatererName = order?.Caterer?.Name ?? ""
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RatingViewModel model)
    {
        var customerId = int.Parse(User.FindFirstValue("CustomerId")!);
        var canRate = await db.Orders.AnyAsync(o => o.Id == model.OrderId && o.CustomerId == customerId && OrderStatus.Rateable.Contains(o.Status) && o.Lines.Any(l => l.MenuItemId == model.MenuItemId));
        if (!canRate) return Forbid();
        var exists = await db.MenuItemReviews.AnyAsync(r => r.OrderId == model.OrderId && r.MenuItemId == model.MenuItemId);
        if (exists)
        {
            ModelState.AddModelError("", "This item has already been rated for the order.");
        }
        if (!ModelState.IsValid)
        {
            await HydrateRatingFormAsync(model);
            return View(model);
        }

        db.MenuItemReviews.Add(new MenuItemReview
        {
            OrderId = model.OrderId,
            MenuItemId = model.MenuItemId,
            ItemRating = model.ItemRating,
            CatererRating = model.CatererRating,
            Comment = model.Comment
        });
        await db.SaveChangesAsync();
        await logs.InfoAsync("Rating.Created", $"Order={model.OrderId}; MenuItem={model.MenuItemId}; Item={model.ItemRating}; Caterer={model.CatererRating}", actorEmail: User.Identity?.Name);
        var catererId = await db.Orders
            .AsNoTracking()
            .Where(o => o.Id == model.OrderId)
            .Select(o => o.CatererId)
            .FirstOrDefaultAsync();
        TempData["Success"] = "Thanks, your rating has been saved.";
        return catererId == 0
            ? RedirectToAction("Details", "Orders", new { id = model.OrderId })
            : RedirectToAction("Index", "Menu", new { catererId });
    }

    private async Task HydrateRatingFormAsync(RatingViewModel model)
    {
        var line = await db.OrderLines
            .Include(l => l.MenuItem)
            .Include(l => l.Order).ThenInclude(o => o!.Caterer)
            .FirstOrDefaultAsync(l => l.OrderId == model.OrderId && l.MenuItemId == model.MenuItemId);

        model.MenuItemName = line?.MenuItem?.Name ?? model.MenuItemName;
        model.CatererName = line?.Order?.Caterer?.Name ?? model.CatererName;
    }
}
