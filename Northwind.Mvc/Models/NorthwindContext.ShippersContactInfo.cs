using Microsoft.EntityFrameworkCore;

namespace Northwind.Mvc.Models;

public partial class NorthwindContext
{
    public virtual DbSet<ShippersContactInfo> ShippersContactInfos { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShippersContactInfo>(entity =>
        {
            entity.HasKey(e => e.ShippersContactInfoId);

            entity.Property(e => e.Address).HasMaxLength(120);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.CustomerId).HasMaxLength(5).IsFixedLength();
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(24);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Website).HasMaxLength(200);

            entity.HasOne(d => d.ShipperNavigation)
                .WithMany()
                .HasForeignKey(d => d.Shipper)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ShippersContactInfos_Shippers");
        });
    }
}
