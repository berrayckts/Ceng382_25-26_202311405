using Northwind.Mvc.Models;

namespace Northwind.Mvc.ViewModels;

public class HomeIndexViewModel
{
    public IReadOnlyList<CatererProfile> FeaturedCaterers { get; set; } = [];
    public IReadOnlyList<MenuItemEntity> SignatureMenus { get; set; } = [];
    public CartViewModel Cart { get; set; } = new();
    public decimal DefaultLatitude { get; set; }
    public decimal DefaultLongitude { get; set; }
}
