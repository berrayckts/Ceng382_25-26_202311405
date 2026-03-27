using Microsoft.EntityFrameworkCore;

namespace Northwind.Mvc.Models;

public partial class NorthwindContext
{
    public virtual DbSet<ContactInfo> ContactInfos { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContactInfo>(entity =>
        {
            entity.HasKey(e => e.ContactInfoId);

            entity.Property(e => e.Address).HasMaxLength(120);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(120);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(24);
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);

            entity.HasOne(d => d.Shipper)
                .WithMany(p => p.ContactInfos)
                .HasForeignKey(d => d.ShipperId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ContactInfos_Shippers");
        });
    }
}
