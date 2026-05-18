using Northwind.Mvc.ViewModels;

namespace Northwind.Mvc.Services;

public interface ICartService
{
    CartViewModel GetCart(ISession session);
    void SaveCart(ISession session, CartViewModel cart);
    void Clear(ISession session);
}
