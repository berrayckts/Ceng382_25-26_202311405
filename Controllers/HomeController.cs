using System.Diagnostics;
using Ceng382_25_26_202311405.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ceng382_25_26_202311405.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToPage("/Account/Register", new { area = "Identity" });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
