using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.ViewModels;

public class MenuIndexViewModel
{
    public PagedResult<MenuItemEntity> MenuItems { get; set; } = new();
    public IReadOnlyList<CatererDistanceViewModel> Caterers { get; set; } = [];
    public CatererDistanceViewModel? SelectedCaterer { get; set; }
    public string? Category { get; set; }
    public decimal? UserLatitude { get; set; }
    public decimal? UserLongitude { get; set; }
    public double RadiusKm { get; set; }
    public bool HasLocationFilter { get; set; }
    public string? Search { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public IReadOnlyList<string> CityOptions { get; set; } = [];
    public IReadOnlyList<string> DistrictOptions { get; set; } = [];
}

public class CatererDistanceViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Category { get; set; }
    public string? ImagePath { get; set; }
    public decimal MinimumPrice { get; set; }
    public double DistanceKm { get; set; }
    public int ActiveMenuCount { get; set; }
    public IReadOnlyList<string> AvailableDistricts { get; set; } = [];
}

public class MenuItemFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = "";

    [Required, StringLength(500)]
    public string Description { get; set; } = "";

    [Range(1, 100000)]
    public decimal BasePrice { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Choose a category.")]
    public int CategoryId { get; set; }

    public bool IsActive { get; set; } = true;
    public IFormFile? Image { get; set; }
    [StringLength(240)]
    public string? ImageUrl { get; set; }
    public string? ExistingImagePath { get; set; }
    public string? RemovableIngredients { get; set; }
    public string? OptionalExtras { get; set; }
    public string? OptionGroupTitle { get; set; }
    public string? OptionGroupValues { get; set; }
    public List<CuisineCategory> Categories { get; set; } = [];
}

public class AddToCartViewModel
{
    public int MenuItemId { get; set; }
    [Range(1, 500)]
    public int Quantity { get; set; } = 1;
    public List<int> SelectedOptionIds { get; set; } = [];
}

public class MenuOptionGroupFormViewModel : IValidatableObject
{
    public int MenuItemId { get; set; }
    public int GroupId { get; set; }

    [Required, StringLength(80)]
    public string Title { get; set; } = "";

    public bool AllowsMultipleSelections { get; set; } = true;
    public bool IsRequired { get; set; }

    [Range(0, 20)]
    public int MinSelection { get; set; }

    [Range(0, 20)]
    public int MaxSelection { get; set; } = 1;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MaxSelection < MinSelection)
        {
            yield return new ValidationResult("Max selection must be greater than or equal to min selection.", [nameof(MaxSelection)]);
        }

        if (IsRequired && MaxSelection < 1)
        {
            yield return new ValidationResult("Required groups must allow at least one selection.", [nameof(MaxSelection)]);
        }

        if (!AllowsMultipleSelections && MaxSelection > 1)
        {
            yield return new ValidationResult("Single choice groups can only allow one selection.", [nameof(MaxSelection)]);
        }
    }
}

public class MenuOptionFormViewModel
{
    public int MenuItemId { get; set; }
    public int GroupId { get; set; }
    public int OptionId { get; set; }

    [Required, StringLength(80)]
    public string Title { get; set; } = "";

    [StringLength(80)]
    public string? QuantityInfo { get; set; }

    [StringLength(24)]
    public string OptionType { get; set; } = MenuItemOptionType.Extra;

    [Range(-100000, 100000)]
    public decimal PriceDelta { get; set; }

    public bool IsDefault { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsRemovable { get; set; }
}

public class CatererProfileFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = "";

    [StringLength(600)]
    public string? BrandStory { get; set; }

    [Required, StringLength(200)]
    public string AddressLine { get; set; } = "";

    [Required, StringLength(80)]
    public string City { get; set; } = "";

    [StringLength(80)]
    public string? District { get; set; }

    [EmailAddress, StringLength(160)]
    public string? Email { get; set; }

    [StringLength(40)]
    public string? Phone { get; set; }

    [Range(-90, 90)]
    public decimal Latitude { get; set; } = 41.0082m;

    [Range(-180, 180)]
    public decimal Longitude { get; set; } = 28.9784m;
}
