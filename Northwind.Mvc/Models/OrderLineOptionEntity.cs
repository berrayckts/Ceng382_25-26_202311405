namespace Northwind.Mvc.Models;

public class OrderLineOptionEntity
{
    public int Id { get; set; }
    public int OrderLineId { get; set; }
    public required string OptionLabel { get; set; }
    public decimal PriceDelta { get; set; }

    public OrderLineEntity? OrderLine { get; set; }
}
