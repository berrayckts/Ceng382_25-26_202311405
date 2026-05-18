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

[Authorize]
public class NearbyController(
    CateringProjectContext db,
    IOptions<CateringPlatformOptions> options,
    ILogService logs,
    ILocationService locations) : Controller
{
    public IActionResult Index()
    {
        var cfg = options.Value;
        return View(new NearbyIndexViewModel
        {
            DefaultLatitude = cfg.DefaultLatitude,
            DefaultLongitude = cfg.DefaultLongitude
        });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Locations(decimal? lat, decimal? lng, string? city, string? district, string? search, double radiusKm = 30, int take = 30)
    {
        var cfg = options.Value;
        var referenceLat = lat ?? cfg.DefaultLatitude;
        var referenceLng = lng ?? cfg.DefaultLongitude;
        var safeRadius = Math.Clamp(radiusKm, 1, 120);
        var safeTake = Math.Clamp(take, 5, 60);
        var hasPlaceFilter = !string.IsNullOrWhiteSpace(city) || !string.IsNullOrWhiteSpace(district) || !string.IsNullOrWhiteSpace(search);

        var query = db.Caterers
                .AsNoTracking()
                .Include(c => c.MenuItems.Where(m => m.IsActive))
                    .ThenInclude(m => m.Category)
                .Include(c => c.MenuItems.Where(m => m.IsActive))
                    .ThenInclude(m => m.OptionGroups)
                        .ThenInclude(g => g.Options)
                .Where(c => c.IsApproved && c.Latitude != 0 && c.Longitude != 0);

        if (!hasPlaceFilter)
        {
            query = locations.Nearby(query, referenceLat, referenceLng, safeRadius);
        }

        var catererEntities = await query.ToListAsync();
        catererEntities = catererEntities
            .Where(c => string.IsNullOrWhiteSpace(city) || TextEquals(c.City, city))
            .Where(c => string.IsNullOrWhiteSpace(district) || TextEquals(c.District, district))
            .Where(c => MatchesSearch(c, search))
            .ToList();

        var ids = catererEntities.Select(c => c.Id).ToList();
        var ratings = await db.MenuItemReviews
            .AsNoTracking()
            .Where(r => r.Order != null && ids.Contains(r.Order.CatererId))
            .GroupBy(r => r.Order!.CatererId)
            .Select(g => new { CatererId = g.Key, Rating = g.Average(r => (double)r.CatererRating) })
            .ToDictionaryAsync(r => r.CatererId, r => r.Rating);

        var result = catererEntities
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.AddressLine,
                c.Latitude,
                c.Longitude,
                Category = c.MenuItems.Select(m => m.Category!.Name).FirstOrDefault() ?? "Catering",
                MinPrice = c.MenuItems.Select(m => (decimal?)m.BasePrice).Min(),
                Rating = ratings.TryGetValue(c.Id, out var rating) ? rating : (double?)null,
                DistanceKm = locations.DistanceKm(referenceLat, referenceLng, c.Latitude, c.Longitude)
            })
            .OrderBy(c => c.DistanceKm)
            .ThenBy(c => c.Name)
            .Take(safeTake)
            .Select(c => new CatererLocationDto(
                c.Id,
                c.Name,
                c.AddressLine,
                c.Category,
                c.Latitude,
                c.Longitude,
                c.Rating.HasValue ? c.Rating.Value.ToString("0.0") : null,
                c.MinPrice.HasValue ? "From " + c.MinPrice.Value.ToString("C") : null,
                c.DistanceKm))
            .ToList();

        return Json(result);
    }

    [AllowAnonymous]
    public async Task<IActionResult> FilterOptions(string? city)
    {
        var caterers = await db.Caterers
            .AsNoTracking()
            .Where(c => c.IsApproved && c.Latitude != 0 && c.Longitude != 0)
            .Select(c => new { c.City, c.District })
            .ToListAsync();

        var cities = caterers
            .Select(c => c.City)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c)
            .ToList();

        var districts = caterers
            .Where(c => string.IsNullOrWhiteSpace(city) || TextEquals(c.City, city))
            .Select(c => c.District)
            .Where(d => !string.IsNullOrWhiteSpace(d))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(d => d)
            .ToList();

        return Json(new { cities, districts });
    }

    [AllowAnonymous]
    public async Task<IActionResult> PlaceFocus(string? city, string? district)
    {
        var caterers = await db.Caterers
            .AsNoTracking()
            .Where(c => c.IsApproved && c.Latitude != 0 && c.Longitude != 0)
            .ToListAsync();

        var matches = caterers
            .Where(c => string.IsNullOrWhiteSpace(city) || TextEquals(c.City, city))
            .Where(c => string.IsNullOrWhiteSpace(district) || TextEquals(c.District, district))
            .ToList();

        if (matches.Count == 0)
        {
            return NotFound();
        }

        return Json(new
        {
            latitude = matches.Average(c => c.Latitude),
            longitude = matches.Average(c => c.Longitude),
            zoom = string.IsNullOrWhiteSpace(district) ? 11 : 13
        });
    }

    public async Task<IActionResult> LocationsOld()
    {
        var caterers = await db.Caterers
            .Include(c => c.MenuItems)
            .Where(c => c.IsApproved && c.Latitude != 0 && c.Longitude != 0)
            .OrderBy(c => c.Name)
            .Select(c => new CatererLocationDto(
                c.Id,
                c.Name,
                c.AddressLine,
                c.MenuItems.Select(m => m.Category!.Name).FirstOrDefault() ?? "Catering",
                c.Latitude,
                c.Longitude,
                c.Orders.SelectMany(o => o.Reviews).Any()
                    ? c.Orders.SelectMany(o => o.Reviews).Average(r => r.CatererRating).ToString("0.0")
                    : null,
                c.MenuItems.Any()
                    ? "From " + c.MenuItems.Min(m => m.BasePrice).ToString("C")
                    : null,
                0))
            .ToListAsync();

        return Json(caterers);
    }

    [Authorize(Roles = AppRole.Admin)]
    [HttpPost]
    public async Task<IActionResult> UpdateCatererLocation([FromBody] UpdateCatererLocationRequest request)
    {
        var caterer = await db.Caterers.FindAsync(request.CatererId);
        if (caterer is null)
        {
            return NotFound(new { message = "Caterer not found." });
        }

        if (request.Latitude is < -90 or > 90 || request.Longitude is < -180 or > 180)
        {
            return BadRequest(new { message = "Invalid coordinates." });
        }

        caterer.Latitude = request.Latitude;
        caterer.Longitude = request.Longitude;
        await db.SaveChangesAsync();
        await logs.InfoAsync("Nearby.CatererLocationUpdated", $"CatererId={caterer.Id}; Lat={caterer.Latitude}; Lng={caterer.Longitude}", actorEmail: User.Identity?.Name);
        return Ok(new { success = true, caterer.Id, caterer.Latitude, caterer.Longitude });
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
