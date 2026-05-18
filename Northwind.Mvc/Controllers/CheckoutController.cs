using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;
using Northwind.Mvc.Services;
using Northwind.Mvc.ViewModels;

namespace Northwind.Mvc.Controllers;

[Authorize(Roles = AppRole.CustomerOrAdmin)]
public class CheckoutController(CateringProjectContext db, ICartService carts, IEmailService email, ILogService logs) : Controller
{
    public async Task<IActionResult> Payment()
    {
        var cart = carts.GetCart(HttpContext.Session);
        if (cart.Items.Count == 0)
        {
            return RedirectToAction("Index", "Cart");
        }

        var customerId = await ResolveCustomerIdAsync();
        var customer = await db.Customers.FindAsync(customerId);
        return View(new PaymentViewModel
        {
            Cart = cart,
            ReceiptEmail = User.Identity?.Name ?? customer?.Email ?? "",
            DeliveryAddress = customer?.City ?? ""
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Payment(PaymentViewModel model)
    {
        var cart = carts.GetCart(HttpContext.Session);
        model.Cart = cart;
        if (cart.Items.Count == 0)
        {
            return RedirectToAction("Index", "Cart");
        }

        var cardholderName = model.CardholderName ?? "";
        var cardNumber = (model.CardNumber ?? "").Replace(" ", "");
        var expiry = model.Expiry ?? "";
        var cvv = model.Cvv ?? "";
        var receiptEmail = model.ReceiptEmail ?? "";
        if (string.IsNullOrWhiteSpace(cardholderName) || cardNumber.Length < 12 || string.IsNullOrWhiteSpace(expiry) || string.IsNullOrWhiteSpace(cvv))
        {
            ModelState.AddModelError("", "Enter demo card details to simulate payment.");
            return View(model);
        }
        if (!string.IsNullOrWhiteSpace(receiptEmail) && !receiptEmail.Contains('@'))
        {
            ModelState.AddModelError(nameof(model.ReceiptEmail), "Enter a valid receipt email address.");
            return View(model);
        }

        var customerId = await ResolveCustomerIdAsync();
        var catererId = cart.Items.First().CatererId;
        if (cart.Items.Any(i => i.CatererId != catererId))
        {
            ModelState.AddModelError("", "A demo order can contain items from one caterer. Please split the cart.");
            return View(model);
        }

        var order = new OrderEntity
        {
            CustomerId = customerId,
            CatererId = catererId,
            Status = OrderStatus.Paid,
            TotalAmount = cart.Total,
            PaymentReference = $"SIM-{DateTime.UtcNow:yyyyMMddHHmmss}",
            DeliveryAddress = model.DeliveryAddress,
            EventNotes = model.EventNotes
        };
        foreach (var item in cart.Items)
        {
            order.Lines.Add(new OrderLineEntity
            {
                MenuItemId = item.MenuItemId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice + item.Options.Sum(o => o.PriceDelta),
                CustomizationSummary = string.Join(", ", item.Options.Select(o => o.Label)),
                AppliedOptions = item.Options.Select(o => new OrderLineOptionEntity { OptionLabel = o.Label, PriceDelta = o.PriceDelta }).ToList()
            });
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync();
        carts.Clear(HttpContext.Session);
        await logs.InfoAsync("Payment.Success", $"Order={order.Id}; Ref={order.PaymentReference}; Total={order.TotalAmount}", actorEmail: User.Identity?.Name);
        await logs.InfoAsync("Order.Created", $"Order={order.Id}; Caterer={order.CatererId}; Customer={order.CustomerId}", actorEmail: User.Identity?.Name);
        try
        {
            await email.SendOrderSummaryAsync(order, receiptEmail);
        }
        catch (Exception ex)
        {
            await logs.ErrorAsync("Checkout.EmailFailed", ex, $"Order={order.Id}; ReceiptEmail={receiptEmail}", actorEmail: User.Identity?.Name);
            TempData["Warning"] = "Order was created, but the email could not be sent. You can still download the PDFs from this page.";
        }

        return RedirectToAction("Details", "Orders", new { id = order.Id });
    }

    private async Task<int> ResolveCustomerIdAsync()
    {
        var customerClaim = User.FindFirstValue("CustomerId");
        if (int.TryParse(customerClaim, out var customerId))
        {
            return customerId;
        }

        var demoCustomer = await db.Customers.FirstOrDefaultAsync(c => c.Email == "user@caterease.local");
        if (demoCustomer is not null)
        {
            return demoCustomer.Id;
        }

        demoCustomer = new CustomerProfile
        {
            DisplayName = "Admin Demo Customer",
            Email = "admin-demo@caterease.local",
            City = "Istanbul",
            Latitude = 41.0082m,
            Longitude = 28.9784m
        };
        db.Customers.Add(demoCustomer);
        await db.SaveChangesAsync();
        return demoCustomer.Id;
    }
}
