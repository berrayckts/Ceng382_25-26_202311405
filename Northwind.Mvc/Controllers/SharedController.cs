using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Controllers;

public class SharedController : Controller
{
    [AllowAnonymous]
    public IActionResult Error(int? code)
    {
        Response.StatusCode = code ?? Response.StatusCode;
        return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }

    [AllowAnonymous]
    public IActionResult PageNotFound()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        return View();
    }
}
