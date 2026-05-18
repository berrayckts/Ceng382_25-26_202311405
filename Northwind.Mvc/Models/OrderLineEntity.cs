namespace Northwind.Mvc.Models;

public class OrderLineEntity
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? CustomizationSummary { get; set; }

    public OrderEntity? Order { get; set; }
    public MenuItemEntity? MenuItem { get; set; }
    public ICollection<OrderLineOptionEntity> AppliedOptions { get; set; } = new List<OrderLineOptionEntity>();
}
