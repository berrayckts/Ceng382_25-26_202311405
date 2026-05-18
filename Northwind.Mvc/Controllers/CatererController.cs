using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;
using Northwind.Mvc.Services;
using Northwind.Mvc.ViewModels;

namespace Northwind.Mvc.Controllers;

[Authorize(Roles = AppRole.RestaurantOwnerOnly)]
public class CatererController(
    CateringProjectContext db,
    ILogService logs,
    IWebHostEnvironment env,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var catererId = await CurrentCatererIdAsync();
        if (catererId is null)
        {
            return RedirectToAction(nameof(Profile));
        }

        var id = catererId.Value;
        var caterer = await db.Caterers.FindAsync(id);
        if (caterer is null)
        {
            return RedirectToAction(nameof(Profile));
        }

        ViewBag.Caterer = caterer;
        ViewBag.ProfileComplete = IsProfileComplete(caterer);
        ViewBag.MenuCount = await db.MenuItems.CountAsync(m => m.CatererId == id);
        ViewBag.AvailableMenuCount = await db.MenuItems.CountAsync(m => m.CatererId == id && m.IsActive);
        ViewBag.OpenOrders = await db.Orders.CountAsync(o => o.CatererId == id && o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled);
        ViewBag.Revenue = await db.Orders.Where(o => o.CatererId == id).SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
        ViewBag.RecentOrders = await db.Orders.Include(o => o.Customer).Where(o => o.CatererId == id).OrderByDescending(o => o.CreatedUtc).Take(5).ToListAsync();
        return View();
    }

    public async Task<IActionResult> Profile()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var caterer = user.CatererProfileId.HasValue
            ? await db.Caterers.FindAsync(user.CatererProfileId.Value)
            : null;

        return View(ToProfileForm(caterer, user));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(CatererProfileFormViewModel model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var caterer = user.CatererProfileId.HasValue
            ? await db.Caterers.FindAsync(user.CatererProfileId.Value)
            : null;

        if (caterer is null)
        {
            caterer = new CatererProfile
            {
                Name = model.Name.Trim(),
                Slug = await CreateUniqueSlugAsync(model.Name),
                Latitude = model.Latitude,
                Longitude = model.Longitude
            };
            db.Caterers.Add(caterer);
            await db.SaveChangesAsync();

            user.CatererProfileId = caterer.Id;
            await userManager.UpdateAsync(user);
            await signInManager.RefreshSignInAsync(user);
        }

        caterer.Name = model.Name.Trim();
        caterer.BrandStory = model.BrandStory?.Trim();
        caterer.AddressLine = model.AddressLine.Trim();
        caterer.City = model.City.Trim();
        caterer.District = model.District?.Trim();
        caterer.Email = string.IsNullOrWhiteSpace(model.Email) ? user.Email : model.Email.Trim().ToLowerInvariant();
        caterer.Phone = model.Phone?.Trim();
        caterer.Latitude = model.Latitude;
        caterer.Longitude = model.Longitude;

        await db.SaveChangesAsync();
        await logs.InfoAsync("Caterer.ProfileSaved", $"Caterer={caterer.Id}", actorEmail: User.Identity?.Name);
        TempData["Success"] = "Restaurant profile saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> MenuItems(string? search, int page = 1)
    {
        var catererId = await CurrentCatererIdAsync();
        if (catererId is null)
        {
            return RedirectToAction(nameof(Profile));
        }

        var id = catererId.Value;
        var query = db.MenuItems.Include(m => m.Category).Include(m => m.OptionGroups).ThenInclude(g => g.Options).Where(m => m.CatererId == id);
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(m => m.Name.Contains(search) || m.Description.Contains(search));
        const int pageSize = 10;
        var total = await query.CountAsync();
        var items = await query.OrderBy(m => m.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return View(new PagedResult<MenuItemEntity> { Items = items, Page = page, PageSize = pageSize, TotalItems = total, Search = search });
    }

    public async Task<IActionResult> CreateMenuItem()
    {
        if (await CurrentCatererIdAsync() is null)
        {
            return RedirectToAction(nameof(Profile));
        }

        return View("MenuItemForm", new MenuItemFormViewModel { Categories = await db.Categories.OrderBy(c => c.Name).ToListAsync() });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMenuItem(MenuItemFormViewModel model)
    {
        var catererId = await CurrentCatererIdAsync();
        if (catererId is null)
        {
            return RedirectToAction(nameof(Profile));
        }

        if (!ModelState.IsValid)
        {
            model.Categories = await db.Categories.OrderBy(c => c.Name).ToListAsync();
            return View("MenuItemForm", model);
        }

        var item = new MenuItemEntity
        {
            CatererId = catererId.Value,
            CategoryId = model.CategoryId,
            Name = model.Name.Trim(),
            Description = model.Description.Trim(),
            BasePrice = model.BasePrice,
            ImagePath = await SaveImageAsync(model.Image) ?? CleanImageUrl(model.ImageUrl),
            IsActive = model.IsActive
        };
        db.MenuItems.Add(item);
        await db.SaveChangesAsync();
        await SaveOptionsAsync(item.Id, model);
        await logs.InfoAsync("Caterer.MenuCreated", $"MenuItem={item.Id}", actorEmail: User.Identity?.Name);
        TempData["Success"] = "Menu item created.";
        return RedirectToAction(nameof(MenuItems));
    }

    public async Task<IActionResult> EditMenuItem(int id)
    {
        var catererId = await CurrentCatererIdAsync();
        if (catererId is null)
        {
            return RedirectToAction(nameof(Profile));
        }

        var catererProfileId = catererId.Value;
        var item = await db.MenuItems.Include(m => m.OptionGroups).ThenInclude(g => g.Options).FirstOrDefaultAsync(m => m.Id == id && m.CatererId == catererProfileId);
        if (item is null) return NotFound();
        return View("MenuItemForm", new MenuItemFormViewModel
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            BasePrice = item.BasePrice,
            CategoryId = item.CategoryId,
            IsActive = item.IsActive,
            ExistingImagePath = item.ImagePath,
            ImageUrl = IsExternalImage(item.ImagePath) ? item.ImagePath : null,
            RemovableIngredients = OptionsToText(item.OptionGroups, "Removable Ingredients"),
            OptionalExtras = OptionsToText(item.OptionGroups, "Optional Extras"),
            OptionGroupTitle = item.OptionGroups.FirstOrDefault(g => g.Title != "Removable Ingredients" && g.Title != "Optional Extras")?.Title,
            OptionGroupValues = OptionsToText(item.OptionGroups.Where(g => g.Title != "Removable Ingredients" && g.Title != "Optional Extras")),
            Categories = await db.Categories.OrderBy(c => c.Name).ToListAsync()
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMenuItem(MenuItemFormViewModel model)
    {
        var catererId = await CurrentCatererIdAsync();
        if (catererId is null)
        {
            return RedirectToAction(nameof(Profile));
        }

        var catererProfileId = catererId.Value;
        var item = await db.MenuItems.FirstOrDefaultAsync(m => m.Id == model.Id && m.CatererId == catererProfileId);
        if (item is null) return NotFound();
        if (!ModelState.IsValid)
        {
            model.Categories = await db.Categories.OrderBy(c => c.Name).ToListAsync();
            return View("MenuItemForm", model);
        }

        item.Name = model.Name.Trim();
        item.Description = model.Description.Trim();
        item.BasePrice = model.BasePrice;
        item.CategoryId = model.CategoryId;
        item.IsActive = model.IsActive;
        item.ImagePath = await SaveImageAsync(model.Image) ?? CleanImageUrl(model.ImageUrl) ?? item.ImagePath;
        await db.SaveChangesAsync();
        await SaveOptionsAsync(item.Id, model);
        await logs.InfoAsync("Caterer.MenuEdited", $"MenuItem={item.Id}", actorEmail: User.Identity?.Name);
        TempData["Success"] = "Menu item updated.";
        return RedirectToAction(nameof(MenuItems));
    }

    public async Task<IActionResult> DeleteMenuItem(int id)
    {
        var catererId = await CurrentCatererIdAsync();
        if (catererId is null)
        {
            return RedirectToAction(nameof(Profile));
        }

        var catererProfileId = catererId.Value;
        var item = await db.MenuItems.Include(m => m.Category).FirstOrDefaultAsync(m => m.Id == id && m.CatererId == catererProfileId);
        if (item is null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("DeleteMenuItem"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMenuItemConfirmed(int id)
    {
        var catererId = await CurrentCatererIdAsync();
        if (catererId is null)
        {
            return RedirectToAction(nameof(Profile));
        }

        var catererProfileId = catererId.Value;
        var item = await db.MenuItems.FirstOrDefaultAsync(m => m.Id == id && m.CatererId == catererProfileId);
        if (item is null) return NotFound();

        if (await db.OrderLines.AnyAsync(l => l.MenuItemId == id))
        {
            item.IsActive = false;
            TempData["Success"] = "This item is used in past orders, so it was marked Not Available instead of being removed.";
        }
        else
        {
            db.MenuItems.Remove(item);
            TempData["Success"] = "Menu item deleted.";
        }

        await db.SaveChangesAsync();
        await logs.InfoAsync("Caterer.MenuDeleted", $"MenuItem={id}", actorEmail: User.Identity?.Name);
        return RedirectToAction(nameof(MenuItems));
    }

    public async Task<IActionResult> ManageOptions(int menuItemId)
    {
        var item = await OwnMenuItemQuery()
            .Include(m => m.Category)
            .Include(m => m.OptionGroups)
            .ThenInclude(g => g.Options)
            .FirstOrDefaultAsync(m => m.Id == menuItemId);

        if (item is null)
        {
            return NotFound();
        }

        item.OptionGroups = item.OptionGroups
            .OrderBy(g => g.Title)
            .Select(g =>
            {
                g.Options = g.Options.OrderBy(o => o.Title).ToList();
                return g;
            })
            .ToList();

        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOptionGroup(MenuOptionGroupFormViewModel model)
    {
        if (!await OwnsMenuItemAsync(model.MenuItemId))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = FirstModelError();
            return RedirectToAction(nameof(ManageOptions), new { menuItemId = model.MenuItemId });
        }

        db.MenuItemOptionGroups.Add(new MenuItemOptionGroup
        {
            MenuItemId = model.MenuItemId,
            Title = model.Title.Trim(),
            AllowsMultipleSelections = model.AllowsMultipleSelections,
            IsRequired = model.IsRequired,
            MinSelection = model.MinSelection,
            MaxSelection = model.MaxSelection
        });

        await db.SaveChangesAsync();
        await logs.InfoAsync("Caterer.OptionGroupCreated", $"MenuItem={model.MenuItemId}", actorEmail: User.Identity?.Name);
        TempData["Success"] = "Option group added.";
        return RedirectToAction(nameof(ManageOptions), new { menuItemId = model.MenuItemId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditOptionGroup(MenuOptionGroupFormViewModel model)
    {
        var group = await OwnOptionGroupQuery(model.MenuItemId).FirstOrDefaultAsync(g => g.Id == model.GroupId);
        if (group is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = FirstModelError();
            return RedirectToAction(nameof(ManageOptions), new { menuItemId = model.MenuItemId });
        }

        group.Title = model.Title.Trim();
        group.AllowsMultipleSelections = model.AllowsMultipleSelections;
        group.IsRequired = model.IsRequired;
        group.MinSelection = model.MinSelection;
        group.MaxSelection = model.MaxSelection;
        await db.SaveChangesAsync();
        TempData["Success"] = "Option group updated.";
        return RedirectToAction(nameof(ManageOptions), new { menuItemId = model.MenuItemId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteOptionGroup(int menuItemId, int groupId)
    {
        var group = await OwnOptionGroupQuery(menuItemId).Include(g => g.Options).FirstOrDefaultAsync(g => g.Id == groupId);
        if (group is null)
        {
            return NotFound();
        }

        db.MenuItemOptionGroups.Remove(group);
        await db.SaveChangesAsync();
        TempData["Success"] = "Option group deleted.";
        return RedirectToAction(nameof(ManageOptions), new { menuItemId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOption(MenuOptionFormViewModel model)
    {
        var group = await OwnOptionGroupQuery(model.MenuItemId).FirstOrDefaultAsync(g => g.Id == model.GroupId);
        if (group is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = FirstModelError();
            return RedirectToAction(nameof(ManageOptions), new { menuItemId = model.MenuItemId });
        }

        group.Options.Add(new MenuItemOption
        {
            Title = model.Title.Trim(),
            QuantityInfo = CleanOptional(model.QuantityInfo),
            OptionType = CleanOptionType(model.OptionType),
            PriceDelta = model.PriceDelta,
            IsDefault = model.IsDefault,
            IsAvailable = model.IsAvailable,
            IsRemovable = model.IsRemovable
        });

        await db.SaveChangesAsync();
        TempData["Success"] = "Option added.";
        return RedirectToAction(nameof(ManageOptions), new { menuItemId = model.MenuItemId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMenuBuilderOption(int menuItemId, string optionType, string title, string? quantityInfo, decimal priceDelta = 0)
    {
        if (!await OwnsMenuItemAsync(menuItemId))
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            TempData["Error"] = "Option name is required.";
            return RedirectToAction(nameof(ManageOptions), new { menuItemId });
        }

        var config = BuilderGroupConfig(optionType);
        var group = await db.MenuItemOptionGroups
            .Include(g => g.Options)
            .FirstOrDefaultAsync(g => g.MenuItemId == menuItemId && g.Title == config.Title);

        if (group is null)
        {
            group = new MenuItemOptionGroup
            {
                MenuItemId = menuItemId,
                Title = config.Title,
                AllowsMultipleSelections = config.Multiple,
                IsRequired = config.Required,
                MinSelection = config.Required ? 1 : 0,
                MaxSelection = config.Max
            };
            db.MenuItemOptionGroups.Add(group);
        }
        else
        {
            group.AllowsMultipleSelections = config.Multiple;
            group.IsRequired = config.Required;
            group.MinSelection = config.Required ? 1 : 0;
            group.MaxSelection = config.Max;
        }

        group.Options.Add(new MenuItemOption
        {
            Title = title.Trim(),
            QuantityInfo = CleanOptional(quantityInfo),
            OptionType = config.Type,
            PriceDelta = config.IsIncluded ? 0 : priceDelta,
            IsDefault = config.IsIncluded,
            IsAvailable = true,
            IsRemovable = config.IsRemovable
        });

        await db.SaveChangesAsync();
        TempData["Success"] = $"{config.Title} option added.";
        return RedirectToAction(nameof(ManageOptions), new { menuItemId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditOption(MenuOptionFormViewModel model)
    {
        var option = await OwnOptionQuery(model.MenuItemId).FirstOrDefaultAsync(o => o.Id == model.OptionId && o.OptionGroupId == model.GroupId);
        if (option is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = FirstModelError();
            return RedirectToAction(nameof(ManageOptions), new { menuItemId = model.MenuItemId });
        }

        option.Title = model.Title.Trim();
        option.QuantityInfo = CleanOptional(model.QuantityInfo);
        option.OptionType = CleanOptionType(model.OptionType);
        option.PriceDelta = model.PriceDelta;
        option.IsDefault = model.IsDefault;
        option.IsAvailable = model.IsAvailable;
        option.IsRemovable = model.IsRemovable;
        await db.SaveChangesAsync();
        TempData["Success"] = "Option updated.";
        return RedirectToAction(nameof(ManageOptions), new { menuItemId = model.MenuItemId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteOption(int menuItemId, int optionId)
    {
        var option = await OwnOptionQuery(menuItemId).FirstOrDefaultAsync(o => o.Id == optionId);
        if (option is null)
        {
            return NotFound();
        }

        db.MenuItemOptions.Remove(option);
        await db.SaveChangesAsync();
        TempData["Success"] = "Option deleted.";
        return RedirectToAction(nameof(ManageOptions), new { menuItemId });
    }

    public IActionResult Orders() => RedirectToAction("Index", "Orders");

    private async Task<int?> CurrentCatererIdAsync()
    {
        if (int.TryParse(User.FindFirstValue("CatererId"), out var catererId))
        {
            return catererId;
        }

        var user = await userManager.GetUserAsync(User);
        return user?.CatererProfileId;
    }

    private async Task<string?> SaveImageAsync(IFormFile? image)
    {
        if (image is null || image.Length == 0) return null;
        var folder = Path.Combine(env.WebRootPath, "uploads", "menu");
        Directory.CreateDirectory(folder);
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var path = Path.Combine(folder, fileName);
        await using var stream = System.IO.File.Create(path);
        await image.CopyToAsync(stream);
        return $"/uploads/menu/{fileName}";
    }

    private static string? CleanImageUrl(string? imageUrl) =>
        string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();

    private async Task SaveOptionsAsync(int menuItemId, MenuItemFormViewModel model)
    {
        var existing = db.MenuItemOptionGroups.Where(g => g.MenuItemId == menuItemId);
        db.MenuItemOptionGroups.RemoveRange(existing);
        await db.SaveChangesAsync();

        AddGroup(menuItemId, "Removable Ingredients", true, false, 0, 4, model.RemovableIngredients, MenuItemOptionType.Removable, removable: true);
        AddGroup(menuItemId, "Optional Extras", true, false, 0, 3, model.OptionalExtras, MenuItemOptionType.Extra, removable: false);
        AddGroup(menuItemId, string.IsNullOrWhiteSpace(model.OptionGroupTitle) ? "Option Group" : model.OptionGroupTitle, false, false, 0, 1, model.OptionGroupValues, MenuItemOptionType.Extra, removable: false);
        await db.SaveChangesAsync();
    }

    private void AddGroup(int itemId, string title, bool multiple, bool required, int minSelection, int maxSelection, string? values, string optionType, bool removable)
    {
        if (string.IsNullOrWhiteSpace(values)) return;
        var group = new MenuItemOptionGroup { MenuItemId = itemId, Title = title, AllowsMultipleSelections = multiple, IsRequired = required, MinSelection = minSelection, MaxSelection = maxSelection };
        foreach (var raw in values.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = raw.Split('|', StringSplitOptions.TrimEntries);
            group.Options.Add(new MenuItemOption
            {
                Title = parts[0],
                PriceDelta = parts.Length > 1 && decimal.TryParse(parts[1], out var price) ? price : 0,
                QuantityInfo = parts.Length > 2 ? CleanOptional(parts[2]) : null,
                OptionType = optionType,
                IsRemovable = removable,
                IsAvailable = true
            });
        }
        db.MenuItemOptionGroups.Add(group);
    }

    private static (string Title, string Type, bool Multiple, bool Required, int Max, bool IsIncluded, bool IsRemovable) BuilderGroupConfig(string? optionType) =>
        optionType?.Trim().ToLowerInvariant() switch
        {
            "included" => ("Included Menu Items", MenuItemOptionType.Included, true, false, 0, true, false),
            "removable" => ("Removable Items", MenuItemOptionType.Removable, true, false, 8, false, true),
            "drink" => ("Drink Choices", MenuItemOptionType.Drink, false, false, 1, false, false),
            _ => ("Add-on / Extra Items", MenuItemOptionType.Extra, true, false, 8, false, false)
        };

    private IQueryable<MenuItemEntity> OwnMenuItemQuery()
    {
        var catererId = CurrentCatererIdAsync().GetAwaiter().GetResult();
        return catererId is null
            ? db.MenuItems.Where(_ => false)
            : db.MenuItems.Where(m => m.CatererId == catererId.Value);
    }

    private IQueryable<MenuItemOptionGroup> OwnOptionGroupQuery(int menuItemId) =>
        db.MenuItemOptionGroups.Where(g => g.MenuItemId == menuItemId && g.MenuItem != null && g.MenuItem.CatererId == CurrentCatererIdAsync().GetAwaiter().GetResult());

    private IQueryable<MenuItemOption> OwnOptionQuery(int menuItemId) =>
        db.MenuItemOptions.Where(o => o.OptionGroup != null && o.OptionGroup.MenuItemId == menuItemId && o.OptionGroup.MenuItem != null && o.OptionGroup.MenuItem.CatererId == CurrentCatererIdAsync().GetAwaiter().GetResult());

    private async Task<bool> OwnsMenuItemAsync(int menuItemId)
    {
        var catererId = await CurrentCatererIdAsync();
        return catererId is not null && await db.MenuItems.AnyAsync(m => m.Id == menuItemId && m.CatererId == catererId.Value);
    }

    private string FirstModelError() =>
        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "Please check the option fields.";

    private static CatererProfileFormViewModel ToProfileForm(CatererProfile? caterer, ApplicationUser user)
    {
        return new CatererProfileFormViewModel
        {
            Id = caterer?.Id ?? 0,
            Name = caterer?.Name ?? user.FullName ?? "",
            BrandStory = caterer?.BrandStory,
            AddressLine = caterer?.AddressLine ?? "",
            City = caterer?.City ?? "",
            District = caterer?.District,
            Email = caterer?.Email ?? user.Email,
            Phone = caterer?.Phone,
            Latitude = caterer?.Latitude == 0 ? 41.0082m : caterer?.Latitude ?? 41.0082m,
            Longitude = caterer?.Longitude == 0 ? 28.9784m : caterer?.Longitude ?? 28.9784m
        };
    }

    private static bool IsProfileComplete(CatererProfile caterer) =>
        !string.IsNullOrWhiteSpace(caterer.Name)
        && !string.IsNullOrWhiteSpace(caterer.AddressLine)
        && !string.IsNullOrWhiteSpace(caterer.City)
        && caterer.Latitude != 0
        && caterer.Longitude != 0;

    private async Task<string> CreateUniqueSlugAsync(string name)
    {
        var baseSlug = string.Join("-", name.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries)).Trim('-');
        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = "caterer";
        }

        var slug = baseSlug;
        var suffix = 2;
        while (await db.Caterers.AnyAsync(c => c.Slug == slug))
        {
            slug = $"{baseSlug}-{suffix++}";
        }

        return slug;
    }

    private static bool IsExternalImage(string? imagePath) =>
        imagePath?.StartsWith("http", StringComparison.OrdinalIgnoreCase) == true;

    private static string? OptionsToText(IEnumerable<MenuItemOptionGroup> groups, string title) =>
        OptionsToText(groups.Where(g => g.Title == title));

    private static string? OptionsToText(IEnumerable<MenuItemOptionGroup> groups)
    {
        var lines = groups
            .SelectMany(g => g.Options)
            .Select(o =>
            {
                var pricePart = o.PriceDelta == 0 ? null : o.PriceDelta.ToString("0.##");
                return string.IsNullOrWhiteSpace(o.QuantityInfo)
                    ? pricePart is null ? o.Title : $"{o.Title}|{pricePart}"
                    : $"{o.Title}|{pricePart ?? "0"}|{o.QuantityInfo}";
            })
            .ToList();

        return lines.Count == 0 ? null : string.Join(Environment.NewLine, lines);
    }

    private static string? CleanOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string CleanOptionType(string? optionType) =>
        optionType?.Trim() switch
        {
            MenuItemOptionType.Included => MenuItemOptionType.Included,
            MenuItemOptionType.Removable => MenuItemOptionType.Removable,
            MenuItemOptionType.Drink => MenuItemOptionType.Drink,
            _ => MenuItemOptionType.Extra
        };
}
