using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;
using Northwind.Mvc.Services;
using Northwind.Mvc.ViewModels;

namespace Northwind.Mvc.Controllers;

[Authorize(Roles = AppRole.CustomerOrAdmin)]
public class CartController(CateringProjectContext db, ICartService carts, ILogService logs) : Controller
{
    public IActionResult Index() => View(carts.GetCart(HttpContext.Session));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddToCartViewModel model)
    {
        var item = await db.MenuItems.Include(m => m.Caterer).Include(m => m.OptionGroups).ThenInclude(g => g.Options).FirstOrDefaultAsync(m => m.Id == model.MenuItemId && m.IsActive);
        if (item is null)
        {
            return NotFound();
        }

        var guestCount = Math.Max(1, model.Quantity);
        var selectedIds = model.SelectedOptionIds.Distinct().ToHashSet();
        var selected = new List<MenuItemOption>();
        foreach (var group in item.OptionGroups)
        {
            var groupSelected = group.Options
                .Where(o => o.IsAvailable && selectedIds.Contains(o.Id))
                .ToList();

            if (group.IsRequired && groupSelected.Count < group.MinSelection)
            {
                TempData["Error"] = $"Please select at least {group.MinSelection} option(s) for {group.Title}.";
                return RedirectToAction("Details", "Menu", new { id = item.Id });
            }

            if (group.MaxSelection > 0 && groupSelected.Count > group.MaxSelection)
            {
                TempData["Error"] = $"Please select at most {group.MaxSelection} option(s) for {group.Title}.";
                return RedirectToAction("Details", "Menu", new { id = item.Id });
            }

            if (!group.AllowsMultipleSelections && groupSelected.Count > 1)
            {
                TempData["Error"] = $"{group.Title} allows only one option.";
                return RedirectToAction("Details", "Menu", new { id = item.Id });
            }

            selected.AddRange(groupSelected);
        }

        var cart = carts.GetCart(HttpContext.Session);
        cart.Items.Add(new CartItemViewModel
        {
            MenuItemId = item.Id,
            CatererId = item.CatererId,
            CatererName = item.Caterer?.Name ?? "",
            Name = item.Name,
            ImagePath = item.ImagePath,
            Quantity = guestCount,
            UnitPrice = item.BasePrice,
            Options = selected.Select(o => new CartOptionViewModel
            {
                OptionId = o.Id,
                Label = string.IsNullOrWhiteSpace(o.QuantityInfo) ? o.Title : $"{o.Title} ({o.QuantityInfo})",
                PriceDelta = o.PriceDelta
            }).ToList()
        });
        carts.SaveCart(HttpContext.Session, cart);
        await logs.InfoAsync("Cart.Add", $"MenuItem={item.Id}; Guests={guestCount}", actorEmail: User.Identity?.Name);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Update(Guid lineId, int quantity)
    {
        var cart = carts.GetCart(HttpContext.Session);
        var line = cart.Items.FirstOrDefault(i => i.CartLineId == lineId);
        if (line is not null)
        {
            line.Quantity = Math.Max(1, quantity);
        }
        carts.SaveCart(HttpContext.Session, cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Remove(Guid lineId)
    {
        var cart = carts.GetCart(HttpContext.Session);
        cart.Items.RemoveAll(i => i.CartLineId == lineId);
        carts.SaveCart(HttpContext.Session, cart);
        return RedirectToAction(nameof(Index));
    }
}
