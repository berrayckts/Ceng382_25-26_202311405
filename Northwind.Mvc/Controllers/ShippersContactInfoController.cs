using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Controllers;

public class ShippersContactInfoController : Controller
{
    private readonly NorthwindContext _context;

    public ShippersContactInfoController(NorthwindContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.ShippersContactInfos
            .Include(x => x.ShipperNavigation)
            .OrderBy(x => x.ShippersContactInfoId)
            .ToListAsync();

        return View(items);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var item = await _context.ShippersContactInfos
            .Include(x => x.ShipperNavigation)
            .FirstOrDefaultAsync(x => x.ShippersContactInfoId == id);

        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    public IActionResult Create()
    {
        PopulateShippers();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ShippersContactInfo item)
    {
        if (!ModelState.IsValid)
        {
            PopulateShippers(item.Shipper);
            return View(item);
        }

        _context.ShippersContactInfos.Add(item);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var item = await _context.ShippersContactInfos.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        PopulateShippers(item.Shipper);
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ShippersContactInfo item)
    {
        if (id != item.ShippersContactInfoId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            PopulateShippers(item.Shipper);
            return View(item);
        }

        try
        {
            _context.Update(item);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.ShippersContactInfos.AnyAsync(x => x.ShippersContactInfoId == item.ShippersContactInfoId))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var item = await _context.ShippersContactInfos
            .Include(x => x.ShipperNavigation)
            .FirstOrDefaultAsync(x => x.ShippersContactInfoId == id);

        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.ShippersContactInfos.FindAsync(id);
        if (item is not null)
        {
            _context.ShippersContactInfos.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private void PopulateShippers(object? selected = null)
    {
        var shippers = _context.Shippers
            .OrderBy(x => x.ShipperId)
            .Select(x => new
            {
                x.ShipperId,
                Text = $"{x.ShipperId} - {x.CompanyName}"
            })
            .ToList();

        ViewBag.Shipper = new SelectList(shippers, "ShipperId", "Text", selected);
    }
}
