namespace Northwind.Mvc.Models;

public class MenuItemOption
{
    public int Id { get; set; }
    public int OptionGroupId { get; set; }
    public required string Title { get; set; }
    public string? QuantityInfo { get; set; }
    public string OptionType { get; set; } = MenuItemOptionType.Extra;
    public decimal PriceDelta { get; set; }
    public bool IsRemovable { get; set; }
    public bool IsDefault { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public MenuItemOptionGroup? OptionGroup { get; set; }
}
