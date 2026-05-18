namespace Northwind.Mvc.Models;

public class CuisineCategory
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string IconKey { get; set; }

    public ICollection<MenuItemEntity> MenuItems { get; set; } = new List<MenuItemEntity>();
}
