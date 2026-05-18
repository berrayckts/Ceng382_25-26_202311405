using Northwind.Mvc.Models;

namespace Northwind.Mvc.Services;

public class LogService(CateringProjectContext db) : ILogService
{
    public Task InfoAsync(string eventName, string? context = null, int? userId = null, string? actorEmail = null) =>
        AddAsync("Info", eventName, context, userId, actorEmail);

    public Task ErrorAsync(string eventName, Exception exception, string? context = null, int? userId = null, string? actorEmail = null) =>
        AddAsync("Error", eventName, $"{context} | {exception.Message}", userId, actorEmail);

    private async Task AddAsync(string level, string eventName, string? context, int? userId, string? actorEmail)
    {
        db.SystemLogs.Add(new SystemLogEntry
        {
            Level = level,
            EventName = eventName,
            ContextData = context,
            UserId = userId,
            ActorEmail = actorEmail
        });
        await db.SaveChangesAsync();
    }
}
