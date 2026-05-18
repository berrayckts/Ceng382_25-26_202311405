namespace Northwind.Mvc.Models;

public class CatererProfile
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? BrandStory { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? AddressLine { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsApproved { get; set; } = true;

    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    public ICollection<MenuItemEntity> MenuItems { get; set; } = new List<MenuItemEntity>();
    public ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
}
