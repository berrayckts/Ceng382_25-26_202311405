namespace Northwind.Mvc.Models;

public class MenuItemOptionGroup
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public required string Title { get; set; }
    public bool AllowsMultipleSelections { get; set; }
    public bool IsRequired { get; set; }
    public int MinSelection { get; set; }
    public int MaxSelection { get; set; }

    public MenuItemEntity? MenuItem { get; set; }
    public ICollection<MenuItemOption> Options { get; set; } = new List<MenuItemOption>();
}
