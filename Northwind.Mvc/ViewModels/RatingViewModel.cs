using System.ComponentModel.DataAnnotations;

namespace Northwind.Mvc.ViewModels;

public class RatingViewModel
{
    public int OrderId { get; set; }
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = "";
    public string CatererName { get; set; } = "";

    [Range(1, 5)]
    public int ItemRating { get; set; } = 5;

    [Range(1, 5)]
    public int CatererRating { get; set; } = 5;

    [StringLength(500)]
    public string? Comment { get; set; }
}
