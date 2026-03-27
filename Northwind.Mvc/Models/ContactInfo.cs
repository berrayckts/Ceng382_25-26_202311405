using System.ComponentModel.DataAnnotations;

namespace Northwind.Mvc.Models;

public class ContactInfo
{
    [Key]
    public int ContactInfoId { get; set; }

    [Display(Name = "Shipper")]
    [Required]
    public int ShipperId { get; set; }

    [EmailAddress]
    [StringLength(120)]
    public string? Email { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Surname { get; set; } = string.Empty;

    [Phone]
    [StringLength(24)]
    public string? Phone { get; set; }

    [StringLength(120)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    [StringLength(50)]
    public string? Position { get; set; }

    public Shipper? Shipper { get; set; }
}
