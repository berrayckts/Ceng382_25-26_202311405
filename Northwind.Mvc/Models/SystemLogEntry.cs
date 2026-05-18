namespace Northwind.Mvc.Models;

public class SystemLogEntry
{
    public int Id { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public required string Level { get; set; }
    public required string EventName { get; set; }
    public int? UserId { get; set; }
    public string? ActorEmail { get; set; }
    public string? ContextData { get; set; }
}
