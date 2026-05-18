namespace Northwind.Mvc.Models;

public static class AvailableDistrictCatalog
{
    private static readonly string[] IstanbulCore = ["Besiktas", "Beyoglu", "Sisli", "Kadikoy", "Uskudar", "Bakirkoy"];
    private static readonly string[] AnkaraCore = ["Cankaya", "Yenimahalle", "Etimesgut", "Sincan", "Kecioren", "Mamak"];

    public static IReadOnlyList<string> For(CatererProfile caterer)
    {
        var homeDistrict = Normalize(caterer.District);
        var city = Normalize(caterer.City);
        var pool = city.Contains("Ankara", StringComparison.OrdinalIgnoreCase) ? AnkaraCore : IstanbulCore;
        var result = new List<string>();

        if (!string.IsNullOrWhiteSpace(homeDistrict))
        {
            result.Add(homeDistrict);
        }

        result.AddRange(pool.Where(d => !result.Contains(d, StringComparer.OrdinalIgnoreCase)).Take(5));
        return result;
    }

    public static IReadOnlyList<string> AllFor(IEnumerable<CatererProfile> caterers) =>
        caterers
            .SelectMany(For)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(d => d)
            .ToList();

    public static bool Serves(CatererProfile caterer, string? district) =>
        string.IsNullOrWhiteSpace(district)
        || For(caterer).Any(d => d.Equals(district.Trim(), StringComparison.OrdinalIgnoreCase));

    private static string Normalize(string? value) => value?.Trim() ?? "";
}
