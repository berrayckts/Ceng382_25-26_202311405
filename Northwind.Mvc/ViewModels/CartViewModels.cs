namespace Northwind.Mvc.ViewModels;

public class CartItemViewModel
{
    public Guid CartLineId { get; set; } = Guid.NewGuid();
    public int MenuItemId { get; set; }
    public int CatererId { get; set; }
    public string CatererName { get; set; } = "";
    public string Name { get; set; } = "";
    public string? ImagePath { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public List<CartOptionViewModel> Options { get; set; } = [];
    public decimal LineTotal => Quantity * (UnitPrice + Options.Sum(o => o.PriceDelta));
}

public class CartOptionViewModel
{
    public int OptionId { get; set; }
    public string Label { get; set; } = "";
    public decimal PriceDelta { get; set; }
}

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = [];
    public decimal Total => Items.Sum(i => i.LineTotal);
}

public class PaymentViewModel
{
    public CartViewModel Cart { get; set; } = new();
    public string CardholderName { get; set; } = "";
    public string CardNumber { get; set; } = "";
    public string Expiry { get; set; } = "";
    public string Cvv { get; set; } = "";
    public string ReceiptEmail { get; set; } = "";
    public string DeliveryAddress { get; set; } = "";
    public string? EventNotes { get; set; }
}
