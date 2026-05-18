namespace Northwind.Mvc.Models;

public class CustomerProfile
{
    public int Id { get; set; }
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public string? City { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    public ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
}
