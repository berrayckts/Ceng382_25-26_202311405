using Microsoft.AspNetCore.Mvc;

namespace Northwind.Mvc.Areas.Owner.Controllers;

public class SettingsController : OwnerBaseController
{
    public IActionResult Index() => RedirectToAction("Profile", "Caterer", new { area = "" });
}
