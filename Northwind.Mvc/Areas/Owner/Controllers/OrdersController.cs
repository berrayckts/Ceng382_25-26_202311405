using Microsoft.AspNetCore.Mvc;

namespace Northwind.Mvc.Areas.Owner.Controllers;

public class OrdersController : OwnerBaseController
{
    public IActionResult Index(string? status) => RedirectToAction("Index", "Orders", new { area = "", status });
    public IActionResult Details(int id) => RedirectToAction("Details", "Orders", new { area = "", id });
}
