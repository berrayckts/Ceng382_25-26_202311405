namespace Northwind.Mvc.Models;

public class MenuItemEntity
{
    public int Id { get; set; }
    public int CatererId { get; set; }
    public int CategoryId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal BasePrice { get; set; }
    public string? ImagePath { get; set; }
    public bool IsActive { get; set; } = true;

    public CatererProfile? Caterer { get; set; }
    public CuisineCategory? Category { get; set; }
    public ICollection<MenuItemOptionGroup> OptionGroups { get; set; } = new List<MenuItemOptionGroup>();
    public ICollection<OrderLineEntity> OrderLines { get; set; } = new List<OrderLineEntity>();
    public ICollection<MenuItemReview> Reviews { get; set; } = new List<MenuItemReview>();
}
