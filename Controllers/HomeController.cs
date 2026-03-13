using System.Diagnostics;
using Ceng382_25_26_202311405.Data;
using Ceng382_25_26_202311405.Models;
using Ceng382_25_26_202311405.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311405.Controllers;

public class HomeController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        var users = await dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Name)
            .ThenBy(user => user.Surname)
            .Select(user => new RegisteredUserViewModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email ?? string.Empty,
                Address = user.Address,
                ProfilePhotoPath = user.ProfilePhotoPath
            })
            .ToListAsync();

        return View(new HomeIndexViewModel
        {
            RegisteredUsers = users
        });
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
