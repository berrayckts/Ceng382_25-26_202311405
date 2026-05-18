using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = AppRole.Admin)]
public abstract class AdminBaseController : Controller
{
}
