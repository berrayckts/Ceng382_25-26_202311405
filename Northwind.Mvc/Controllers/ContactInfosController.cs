using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Controllers;

public class ContactInfosController : Controller
{
    private readonly NorthwindContext _context;

    public ContactInfosController(NorthwindContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var contactInfos = await _context.ContactInfos
            .Include(c => c.Shipper)
            .OrderBy(c => c.ContactInfoId)
            .ToListAsync();

        return View(contactInfos);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var contactInfo = await _context.ContactInfos
            .Include(c => c.Shipper)
            .FirstOrDefaultAsync(m => m.ContactInfoId == id);

        if (contactInfo is null)
        {
            return NotFound();
        }

        return View(contactInfo);
    }

    public IActionResult Create()
    {
        PopulateShippersDropDownList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContactInfo contactInfo)
    {
        if (ModelState.IsValid)
        {
            _context.Add(contactInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        PopulateShippersDropDownList(contactInfo.ShipperId);
        return View(contactInfo);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var contactInfo = await _context.ContactInfos.FindAsync(id);
        if (contactInfo is null)
        {
            return NotFound();
        }

        PopulateShippersDropDownList(contactInfo.ShipperId);
        return View(contactInfo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ContactInfo contactInfo)
    {
        if (id != contactInfo.ContactInfoId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            PopulateShippersDropDownList(contactInfo.ShipperId);
            return View(contactInfo);
        }

        try
        {
            _context.Update(contactInfo);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ContactInfoExists(contactInfo.ContactInfoId))
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

        var contactInfo = await _context.ContactInfos
            .Include(c => c.Shipper)
            .FirstOrDefaultAsync(m => m.ContactInfoId == id);

        if (contactInfo is null)
        {
            return NotFound();
        }

        return View(contactInfo);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var contactInfo = await _context.ContactInfos.FindAsync(id);
        if (contactInfo is not null)
        {
            _context.ContactInfos.Remove(contactInfo);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> ContactInfoExists(int id)
    {
        return await _context.ContactInfos.AnyAsync(e => e.ContactInfoId == id);
    }

    private void PopulateShippersDropDownList(object? selectedShipper = null)
    {
        var shippers = _context.Shippers
            .OrderBy(s => s.CompanyName)
            .Select(s => new
            {
                s.ShipperId,
                DisplayText = $"{s.CompanyName} ({s.Phone ?? "No phone"})"
            })
            .ToList();

        ViewBag.ShipperId = new SelectList(shippers, "ShipperId", "DisplayText", selectedShipper);
    }
}
