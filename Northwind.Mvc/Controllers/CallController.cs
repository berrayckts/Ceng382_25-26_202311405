using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Controllers;

[Authorize]
public class CallController : Controller
{
    public IActionResult Room(int orderId)
    {
        ViewBag.OrderId = orderId;
        ViewBag.RoomName = $"order-{orderId}";
        return View();
    }
}
