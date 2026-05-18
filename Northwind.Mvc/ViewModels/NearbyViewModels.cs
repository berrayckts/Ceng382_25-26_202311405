using Northwind.Mvc.Models;

namespace Northwind.Mvc.ViewModels;

public class NearbyIndexViewModel
{
    public decimal DefaultLatitude { get; set; }
    public decimal DefaultLongitude { get; set; }
}

public class AdminLinkRestaurantsViewModel : NearbyIndexViewModel
{
    public IReadOnlyList<CatererProfile> Caterers { get; set; } = [];
}

public record CatererLocationDto(
    int Id,
    string Name,
    string? Address,
    string? Category,
    decimal Latitude,
    decimal Longitude,
    string? Rating,
    string? PriceInfo,
    double DistanceKm);

public class UpdateCatererLocationRequest
{
    public int CatererId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}
