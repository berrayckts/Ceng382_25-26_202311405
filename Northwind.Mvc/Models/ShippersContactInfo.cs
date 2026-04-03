using System.ComponentModel.DataAnnotations;

namespace Northwind.Mvc.Models;

public class ShippersContactInfo
{
    [Key]
    public int ShippersContactInfoId { get; set; }

    [Required]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Website { get; set; } = string.Empty;

    [StringLength(24)]
    public string? Phone { get; set; }

    [StringLength(120)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    [StringLength(50)]
    public string? Country { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [Required]
    public int Shipper { get; set; }

    [StringLength(5)]
    public string? CustomerId { get; set; }

    public Shipper? ShipperNavigation { get; set; }
}
