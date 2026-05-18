namespace Northwind.Mvc.Models;

public class OrderEntity
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int CatererId { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public required string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string? PaymentReference { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? EventNotes { get; set; }
    public DateTime? CompletedUtc { get; set; }

    public CustomerProfile? Customer { get; set; }
    public CatererProfile? Caterer { get; set; }
    public ICollection<OrderLineEntity> Lines { get; set; } = new List<OrderLineEntity>();
    public ICollection<MenuItemReview> Reviews { get; set; } = new List<MenuItemReview>();
}
