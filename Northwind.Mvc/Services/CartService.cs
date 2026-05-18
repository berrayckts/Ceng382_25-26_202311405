using System.Text.Json;
using Northwind.Mvc.ViewModels;

namespace Northwind.Mvc.Services;

public class CartService : ICartService
{
    private const string CartKey = "Catera.Cart";

    public CartViewModel GetCart(ISession session)
    {
        var json = session.GetString(CartKey);
        return string.IsNullOrWhiteSpace(json)
            ? new CartViewModel()
            : JsonSerializer.Deserialize<CartViewModel>(json) ?? new CartViewModel();
    }

    public void SaveCart(ISession session, CartViewModel cart) =>
        session.SetString(CartKey, JsonSerializer.Serialize(cart));

    public void Clear(ISession session) => session.Remove(CartKey);
}
