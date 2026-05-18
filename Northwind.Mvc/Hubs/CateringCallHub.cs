using Microsoft.AspNetCore.SignalR;

namespace Northwind.Mvc.Hubs;

public class CateringCallHub : Hub
{
    public async Task JoinRoom(string room)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, room);
        await Clients.Group(room).SendAsync("RoomMessage", $"{Context.User?.Identity?.Name ?? "Guest"} joined the room.");
    }

    public Task SendSignal(string room, string payload) =>
        Clients.OthersInGroup(room).SendAsync("ReceiveSignal", payload);

    public Task SendChat(string room, string message) =>
        Clients.Group(room).SendAsync("RoomMessage", $"{Context.User?.Identity?.Name}: {message}");
}
