using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Areas.Owner.Controllers;

[Area("Owner")]
[Authorize(Roles = AppRole.RestaurantOwnerOnly)]
public abstract class OwnerBaseController : Controller
{
    protected int GetOwnerRestaurantId()
    {
        var claim = User.FindFirstValue("OwnedRestaurantId");
        if (!int.TryParse(claim, out var id))
        {
            throw new UnauthorizedAccessException();
        }

        return id;
    }
}
