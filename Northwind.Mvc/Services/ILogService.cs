namespace Northwind.Mvc.Services;

public interface ILogService
{
    Task InfoAsync(string eventName, string? context = null, int? userId = null, string? actorEmail = null);
    Task ErrorAsync(string eventName, Exception exception, string? context = null, int? userId = null, string? actorEmail = null);
}
