using Northwind.Mvc.Models;

namespace Northwind.Mvc.Services;

public interface ILocationService
{
    double DistanceKm(decimal lat1, decimal lon1, decimal lat2, decimal lon2);
    IQueryable<CatererProfile> Nearby(IQueryable<CatererProfile> caterers, decimal latitude, decimal longitude, double radiusKm);
}
