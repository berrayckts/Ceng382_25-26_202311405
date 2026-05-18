namespace Northwind.Mvc.ViewModels;

public class GuestPriceCalculatorViewModel
{
    public decimal PricePerPerson { get; set; }
    public decimal SelectedAddonsTotal { get; set; }
    public int InitialGuestCount { get; set; } = 1;
    public string InputName { get; set; } = "Quantity";
    public bool ShowInput { get; set; } = true;
}

public class AvailableDistrictsViewModel
{
    public IReadOnlyList<string> Districts { get; set; } = [];
    public int MaxVisible { get; set; } = 4;
    public string Variant { get; set; } = "inline";
}
