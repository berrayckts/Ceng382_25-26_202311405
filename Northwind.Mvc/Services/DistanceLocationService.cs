using Northwind.Mvc.Models;

namespace Northwind.Mvc.Services;

public class DistanceLocationService : ILocationService
{
    public double DistanceKm(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        const double earthRadiusKm = 6371;
        static double Rad(decimal value) => (double)value * Math.PI / 180;
        var dLat = Rad(lat2 - lat1);
        var dLon = Rad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
            + Math.Cos(Rad(lat1)) * Math.Cos(Rad(lat2))
            * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    public IQueryable<CatererProfile> Nearby(IQueryable<CatererProfile> caterers, decimal latitude, decimal longitude, double radiusKm)
    {
        var latWindow = (decimal)(radiusKm / 111d);
        var lonWindow = (decimal)(radiusKm / 85d);
        return caterers.Where(c =>
            c.Latitude >= latitude - latWindow &&
            c.Latitude <= latitude + latWindow &&
            c.Longitude >= longitude - lonWindow &&
            c.Longitude <= longitude + lonWindow);
    }
}
