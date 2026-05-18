using Microsoft.AspNetCore.Mvc;

namespace Northwind.Mvc.Areas.Owner.Controllers;

public class MenuController : OwnerBaseController
{
    public IActionResult Index() => RedirectToAction("MenuItems", "Caterer", new { area = "" });
    public IActionResult Edit(int? id) => id.HasValue
        ? RedirectToAction("EditMenuItem", "Caterer", new { area = "", id = id.Value })
        : RedirectToAction("CreateMenuItem", "Caterer", new { area = "" });
}
