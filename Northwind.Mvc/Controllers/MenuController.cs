using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Northwind.Mvc.Models;
using Northwind.Mvc.Services;
using Northwind.Mvc.ViewModels;
using System.Globalization;
using System.Text;

namespace Northwind.Mvc.Controllers;

public class MenuController(CateringProjectContext db, ILocationService locations, IOptions<CateringPlatformOptions> options) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index(string? search, string? category, string? city, string? district, int? catererId, int? guests, string? lat, string? lng, double? radiusKm, int page = 1)
    {
        var cfg = options.Value;
        var parsedLat = ParseCoordinate(lat, -90m, 90m);
        var parsedLng = ParseCoordinate(lng, -180m, 180m);
        var activeRadiusKm = Math.Clamp(radiusKm ?? cfg.NearbyRadiusKm, 1, 100);
        var hasLocationFilter = parsedLat.HasValue && parsedLng.HasValue;
        var referenceLat = parsedLat ?? cfg.DefaultLatitude;
        var referenceLng = parsedLng ?? cfg.DefaultLongitude;
        var caterers = db.Caterers
            .AsNoTracking()
            .Include(c => c.MenuItems.Where(m => m.IsActive))
                .ThenInclude(m => m.Category)
            .Include(c => c.MenuItems.Where(m => m.IsActive))
                .ThenInclude(m => m.OptionGroups)
                    .ThenInclude(g => g.Options)
            .Where(c => c.IsApproved);

        if (hasLocationFilter)
        {
            caterers = locations.Nearby(caterers, referenceLat, referenceLng, activeRadiusKm);
        }

        var catererList = await caterers.ToListAsync();
        var cityOptions = catererList
            .Select(c => c.City)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c)
            .ToList()!;
        if (!string.IsNullOrWhiteSpace(city))
        {
            catererList = catererList
                .Where(c => TextEquals(c.City, city))
                .ToList();
        }
        var districtOptions = AvailableDistrictCatalog.AllFor(catererList);
        var catererCards = catererList
            .Where(c => AvailableDistrictCatalog.Serves(c, district))
            .Where(c => MatchesSearch(c, search))
            .Select(c => new CatererDistanceViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.AddressLine,
                City = c.City,
                District = c.District,
                Category = c.MenuItems.Select(m => m.Category!.Name).FirstOrDefault() ?? "Catering",
                ImagePath = c.MenuItems.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.ImagePath))?.ImagePath,
                MinimumPrice = c.MenuItems.Count != 0 ? c.MenuItems.Min(m => m.BasePrice) : 0,
                ActiveMenuCount = c.MenuItems.Count,
                DistanceKm = locations.DistanceKm(referenceLat, referenceLng, c.Latitude, c.Longitude),
                AvailableDistricts = AvailableDistrictCatalog.For(c)
            })
            .OrderBy(c => c.DistanceKm)
            .ThenBy(c => c.Name)
            .ToList();

        var orderedCatererIds = catererCards.Select(c => c.Id).ToList();
        var distanceLookup = catererCards.ToDictionary(c => c.Id, c => c.DistanceKm);
        var catererRatingLookup = await db.MenuItemReviews
            .AsNoTracking()
            .Where(r => r.Order != null && orderedCatererIds.Contains(r.Order.CatererId))
            .GroupBy(r => r.Order!.CatererId)
            .Select(g => new { CatererId = g.Key, Rating = g.Average(r => r.CatererRating) })
            .ToDictionaryAsync(r => r.CatererId, r => r.Rating.ToString("0.0"));

        var query = db.MenuItems
            .AsNoTracking()
            .Include(m => m.Caterer)
            .Include(m => m.Category)
            .Include(m => m.Reviews)
            .Include(m => m.OptionGroups).ThenInclude(g => g.Options)
            .Where(m => m.IsActive && orderedCatererIds.Contains(m.CatererId));

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(m => m.Category!.Slug == category);
        }
        if (catererId.HasValue)
        {
            query = query.Where(m => m.CatererId == catererId.Value);
        }

        const int pageSize = 9;
        var allItems = await query.ToListAsync();
        if (!string.IsNullOrWhiteSpace(search))
        {
            allItems = allItems
                .Where(m => MatchesSearch(m, search))
                .ToList();
        }
        var total = allItems.Count;
        var items = allItems
            .OrderBy(m => distanceLookup.GetValueOrDefault(m.CatererId, double.MaxValue))
            .ThenBy(m => m.Caterer!.Name)
            .ThenBy(m => m.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CatererRatings = catererRatingLookup;
        return View(new MenuIndexViewModel
        {
            Caterers = catererCards,
            SelectedCaterer = catererId.HasValue ? catererCards.FirstOrDefault(c => c.Id == catererId.Value) : null,
            Category = category,
            RadiusKm = activeRadiusKm,
            UserLatitude = hasLocationFilter ? referenceLat : null,
            UserLongitude = hasLocationFilter ? referenceLng : null,
            HasLocationFilter = hasLocationFilter,
            Search = search,
            City = city,
            District = district,
            CityOptions = cityOptions,
            DistrictOptions = districtOptions,
            MenuItems = new PagedResult<MenuItemEntity> { Items = items, Page = page, PageSize = pageSize, TotalItems = total, Search = search }
        });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var item = await db.MenuItems
            .Include(m => m.Caterer).ThenInclude(c => c!.Orders).ThenInclude(o => o.Reviews)
            .Include(m => m.Category)
            .Include(m => m.Reviews).ThenInclude(r => r.Order).ThenInclude(o => o!.Customer)
            .Include(m => m.OptionGroups).ThenInclude(g => g.Options)
            .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);
        return item is null ? NotFound() : View(item);
    }

    [Authorize(Roles = AppRole.CustomerOnly)]
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult AddToCart(int id) => RedirectToAction("Details", new { id });

    private static decimal? ParseCoordinate(string? value, decimal min, decimal max)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        var styles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
        var cultures = new[] { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("tr-TR") };

        foreach (var culture in cultures)
        {
            if (decimal.TryParse(normalized, styles, culture, out var coordinate) && coordinate >= min && coordinate <= max)
            {
                return coordinate;
            }
        }

        normalized = normalized.Replace(',', '.');
        return decimal.TryParse(normalized, styles, CultureInfo.InvariantCulture, out var fallback)
            && fallback >= min
            && fallback <= max
            ? fallback
            : null;
    }

    private static bool MatchesSearch(CatererProfile caterer, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return true;
        }

        var needle = NormalizeText(search);
        return NormalizeText(caterer.Name).Contains(needle)
            || NormalizeText(caterer.BrandStory).Contains(needle)
            || caterer.MenuItems.Any(m =>
                NormalizeText(m.Name).Contains(needle)
                || NormalizeText(m.Description).Contains(needle)
                || NormalizeText(m.Category?.Name).Contains(needle)
                || m.OptionGroups.Any(g =>
                    NormalizeText(g.Title).Contains(needle)
                    || g.Options.Any(o =>
                        NormalizeText(o.Title).Contains(needle)
                        || NormalizeText(o.QuantityInfo).Contains(needle))));
    }

    private static bool MatchesSearch(MenuItemEntity item, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return true;
        }

        var needle = NormalizeText(search);
        return NormalizeText(item.Name).Contains(needle)
            || NormalizeText(item.Description).Contains(needle)
            || NormalizeText(item.Caterer?.Name).Contains(needle)
            || NormalizeText(item.Category?.Name).Contains(needle)
            || item.OptionGroups.Any(g =>
                NormalizeText(g.Title).Contains(needle)
                || g.Options.Any(o =>
                    NormalizeText(o.Title).Contains(needle)
                    || NormalizeText(o.QuantityInfo).Contains(needle)));
    }

    private static bool TextEquals(string? value, string? other) =>
        NormalizeText(value) == NormalizeText(other);

    private static string NormalizeText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "";
        }

        var normalized = value.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            builder.Append(character switch
            {
                'ı' => 'i',
                'ğ' => 'g',
                'ü' => 'u',
                'ş' => 's',
                'ö' => 'o',
                'ç' => 'c',
                _ => character
            });
        }

        return builder.ToString();
    }
}
