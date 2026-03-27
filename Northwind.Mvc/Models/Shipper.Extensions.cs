namespace Northwind.Mvc.Models;

public partial class Shipper
{
    public virtual ICollection<ContactInfo> ContactInfos { get; set; } = new List<ContactInfo>();
}
