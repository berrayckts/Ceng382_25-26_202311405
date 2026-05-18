namespace Northwind.Mvc.Models;

public class MenuItemReview
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int MenuItemId { get; set; }
    public int ItemRating { get; set; }
    public int CatererRating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public OrderEntity? Order { get; set; }
    public MenuItemEntity? MenuItem { get; set; }
}
