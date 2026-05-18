using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Northwind.Mvc.Models;

public class CateringProjectContext(DbContextOptions<CateringProjectContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>(options)
{
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<CatererProfile> Caterers => Set<CatererProfile>();
    public DbSet<CustomerProfile> Customers => Set<CustomerProfile>();
    public DbSet<CuisineCategory> Categories => Set<CuisineCategory>();
    public DbSet<MenuItemEntity> MenuItems => Set<MenuItemEntity>();
    public DbSet<MenuItemOptionGroup> MenuItemOptionGroups => Set<MenuItemOptionGroup>();
    public DbSet<MenuItemOption> MenuItemOptions => Set<MenuItemOption>();
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderLineEntity> OrderLines => Set<OrderLineEntity>();
    public DbSet<OrderLineOptionEntity> OrderLineOptions => Set<OrderLineOptionEntity>();
    public DbSet<MenuItemReview> MenuItemReviews => Set<MenuItemReview>();
    public DbSet<SystemLogEntry> SystemLogs => Set<SystemLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.FullName).HasMaxLength(120);
            entity.Property(u => u.EmailVerificationCode).HasMaxLength(6);
            entity.Property(u => u.IsEmailVerified).HasDefaultValue(true);
            entity.HasOne(u => u.CatererProfile)
                .WithMany()
                .HasForeignKey(u => u.CatererProfileId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(u => u.CustomerProfile)
                .WithMany()
                .HasForeignKey(u => u.CustomerProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("AppUsers");
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.FullName).HasMaxLength(120);
            entity.Property(u => u.Email).HasMaxLength(160);
            entity.Property(u => u.PasswordHash).HasMaxLength(500);
            entity.Property(u => u.Role).HasMaxLength(30);
            entity.Property(u => u.TwoFactorCode).HasMaxLength(12);

            entity.HasOne(u => u.CatererProfile)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CatererProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(u => u.CustomerProfile)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CustomerProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CatererProfile>(entity =>
        {
            entity.ToTable("Caterers");
            entity.HasIndex(c => c.Slug).IsUnique();
            entity.Property(c => c.Name).HasMaxLength(120);
            entity.Property(c => c.City).HasMaxLength(80);
            entity.Property(c => c.District).HasMaxLength(80);
            entity.Property(c => c.BrandStory).HasMaxLength(600);
            entity.Property(c => c.AddressLine).HasMaxLength(200);
            entity.Property(c => c.Email).HasMaxLength(160);
            entity.Property(c => c.Phone).HasMaxLength(40);
            entity.Property(c => c.Latitude).HasPrecision(9, 6);
            entity.Property(c => c.Longitude).HasPrecision(9, 6);
        });

        modelBuilder.Entity<CustomerProfile>(entity =>
        {
            entity.ToTable("Customers");
            entity.Property(c => c.DisplayName).HasMaxLength(120);
            entity.Property(c => c.City).HasMaxLength(80);
            entity.Property(c => c.Email).HasMaxLength(140);
            entity.Property(c => c.Latitude).HasPrecision(9, 6);
            entity.Property(c => c.Longitude).HasPrecision(9, 6);
        });

        modelBuilder.Entity<CuisineCategory>(entity =>
        {
            entity.ToTable("CuisineCategories");
            entity.HasIndex(c => c.Slug).IsUnique();
            entity.Property(c => c.Name).HasMaxLength(80);
            entity.Property(c => c.IconKey).HasMaxLength(40);
        });

        modelBuilder.Entity<MenuItemEntity>(entity =>
        {
            entity.ToTable("MenuItems");
            entity.Property(m => m.Name).HasMaxLength(120);
            entity.Property(m => m.Description).HasMaxLength(500);
            entity.Property(m => m.ImagePath).HasMaxLength(240);
            entity.Property(m => m.BasePrice).HasColumnType("decimal(10,2)");

            entity.HasOne(m => m.Caterer)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(m => m.CatererId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MenuItemOptionGroup>(entity =>
        {
            entity.ToTable("MenuItemOptionGroups");
            entity.Property(g => g.Title).HasMaxLength(80);
            entity.Property(g => g.MinSelection).HasDefaultValue(0);
            entity.Property(g => g.MaxSelection).HasDefaultValue(0);

            entity.HasOne(g => g.MenuItem)
                .WithMany(m => m.OptionGroups)
                .HasForeignKey(g => g.MenuItemId);
        });

        modelBuilder.Entity<MenuItemOption>(entity =>
        {
            entity.ToTable("MenuItemOptions");
            entity.Property(o => o.Title).HasMaxLength(80);
            entity.Property(o => o.QuantityInfo).HasMaxLength(80);
            entity.Property(o => o.OptionType).HasMaxLength(24).HasDefaultValue(MenuItemOptionType.Extra);
            entity.Property(o => o.PriceDelta).HasColumnType("decimal(10,2)");
            entity.Property(o => o.IsDefault).HasDefaultValue(false);
            entity.Property(o => o.IsAvailable).HasDefaultValue(true);
            entity.Property(o => o.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(o => o.OptionGroup)
                .WithMany(g => g.Options)
                .HasForeignKey(o => o.OptionGroupId);
        });

        modelBuilder.Entity<OrderEntity>(entity =>
        {
            entity.ToTable("Orders");
            entity.Property(o => o.Status).HasMaxLength(40);
            entity.Property(o => o.PaymentReference).HasMaxLength(80);
            entity.Property(o => o.DeliveryAddress).HasMaxLength(240);
            entity.Property(o => o.EventNotes).HasMaxLength(600);
            entity.Property(o => o.TotalAmount).HasColumnType("decimal(10,2)");

            entity.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Caterer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CatererId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderLineEntity>(entity =>
        {
            entity.ToTable("OrderLines");
            entity.Property(o => o.UnitPrice).HasColumnType("decimal(10,2)");
            entity.Property(o => o.CustomizationSummary).HasMaxLength(600);

            entity.HasOne(o => o.Order)
                .WithMany(or => or.Lines)
                .HasForeignKey(o => o.OrderId);

            entity.HasOne(o => o.MenuItem)
                .WithMany(m => m.OrderLines)
                .HasForeignKey(o => o.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderLineOptionEntity>(entity =>
        {
            entity.ToTable("OrderLineOptions");
            entity.Property(o => o.PriceDelta).HasColumnType("decimal(10,2)");
            entity.Property(o => o.OptionLabel).HasMaxLength(80);

            entity.HasOne(o => o.OrderLine)
                .WithMany(l => l.AppliedOptions)
                .HasForeignKey(o => o.OrderLineId);
        });

        modelBuilder.Entity<MenuItemReview>(entity =>
        {
            entity.ToTable("MenuItemReviews");
            entity.Property(r => r.Comment).HasMaxLength(500);

            entity.HasOne(r => r.Order)
                .WithMany(o => o.Reviews)
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.MenuItem)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SystemLogEntry>(entity =>
        {
            entity.ToTable("SystemLogs");
            entity.Property(l => l.Level).HasMaxLength(20);
            entity.Property(l => l.EventName).HasMaxLength(80);
            entity.Property(l => l.ActorEmail).HasMaxLength(160);
            entity.Property(l => l.ContextData).HasMaxLength(1200);
        });

        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatererProfile>().HasData(
            new CatererProfile
            {
                Id = 1,
                Name = "Istanbul Garden Catering",
                Slug = "istanbul-garden",
                BrandStory = "Seasonal event catering with polished mezze tables, warm mains, and careful service.",
                City = "Istanbul",
                District = "Besiktas",
                AddressLine = "Besiktas, Istanbul",
                Email = "caterer@caterease.local",
                Phone = "+90 212 000 00 00",
                Latitude = 41.0430m,
                Longitude = 29.0094m,
                IsApproved = true
            },
            new CatererProfile
            {
                Id = 101,
                Name = "Galata Banquet Studio",
                Slug = "galata-banquet-studio",
                BrandStory = "Golden-hour tasting menus and refined corporate catering near Galata.",
                City = "Istanbul",
                District = "Beyoglu",
                AddressLine = "Galata, Beyoglu, Istanbul",
                Email = "galata@catera.local",
                Phone = "+90 212 111 22 33",
                Latitude = 41.0256m,
                Longitude = 28.9744m,
                IsApproved = true
            },
            new CatererProfile
            {
                Id = 102,
                Name = "Nisantasi Private Table",
                Slug = "nisantasi-private-table",
                BrandStory = "Boutique dinner service and elegant celebration menus around Nisantasi.",
                City = "Istanbul",
                District = "Sisli",
                AddressLine = "Nisantasi, Sisli, Istanbul",
                Email = "nisantasi@catera.local",
                Phone = "+90 212 222 44 55",
                Latitude = 41.0504m,
                Longitude = 28.9910m,
                IsApproved = true
            });

        modelBuilder.Entity<CustomerProfile>().HasData(new CustomerProfile
        {
            Id = 1,
            DisplayName = "Demo User",
            Email = "user@caterease.local",
            City = "Istanbul",
            Latitude = 41.0082m,
            Longitude = 28.9784m
        });

        modelBuilder.Entity<CuisineCategory>().HasData(
            new CuisineCategory { Id = 1, Name = "Corporate Lunch", Slug = "corporate-lunch", IconKey = "briefcase" },
            new CuisineCategory { Id = 2, Name = "Wedding Table", Slug = "wedding-table", IconKey = "sparkles" },
            new CuisineCategory { Id = 3, Name = "Vegan Feast", Slug = "vegan-feast", IconKey = "leaf" });

        modelBuilder.Entity<MenuItemEntity>().HasData(
            new MenuItemEntity { Id = 1, CatererId = 1, CategoryId = 1, Name = "Executive Mezze Box", Description = "A polished mezze selection with warm flatbread, salads, and dips for office events.", BasePrice = 720, ImagePath = "/images/menu/mezze.svg", IsActive = true },
            new MenuItemEntity { Id = 2, CatererId = 1, CategoryId = 2, Name = "Rosemary Lamb Service", Description = "Slow-roasted lamb with seasonal vegetables and event-ready plating.", BasePrice = 1080, ImagePath = "/images/menu/lamb.svg", IsActive = true },
            new MenuItemEntity { Id = 3, CatererId = 1, CategoryId = 3, Name = "Green Garden Banquet", Description = "Plant-forward mains, grains, and bright sauces for mixed guest lists.", BasePrice = 660, ImagePath = "/images/menu/vegan.svg", IsActive = true });

        modelBuilder.Entity<MenuItemOptionGroup>().HasData(
            new MenuItemOptionGroup { Id = 1, MenuItemId = 1, Title = "Removable Ingredients", AllowsMultipleSelections = true, IsRequired = false, MinSelection = 0, MaxSelection = 3 },
            new MenuItemOptionGroup { Id = 2, MenuItemId = 1, Title = "Optional Extras", AllowsMultipleSelections = true, IsRequired = false, MinSelection = 0, MaxSelection = 3 },
            new MenuItemOptionGroup { Id = 3, MenuItemId = 2, Title = "Service Style", AllowsMultipleSelections = false, IsRequired = true, MinSelection = 1, MaxSelection = 1 });

        modelBuilder.Entity<MenuItemOption>().HasData(
            new MenuItemOption { Id = 1, OptionGroupId = 1, Title = "Remove walnuts", OptionType = MenuItemOptionType.Removable, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PriceDelta = 0, IsRemovable = true, IsDefault = false, IsAvailable = true },
            new MenuItemOption { Id = 2, OptionGroupId = 1, Title = "Remove dairy sauce", OptionType = MenuItemOptionType.Removable, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PriceDelta = 0, IsRemovable = true, IsDefault = false, IsAvailable = true },
            new MenuItemOption { Id = 3, OptionGroupId = 2, Title = "Add dessert bites", OptionType = MenuItemOptionType.Extra, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PriceDelta = 95, IsRemovable = false, IsDefault = false, IsAvailable = true },
            new MenuItemOption { Id = 4, OptionGroupId = 2, Title = "Add premium drinks", OptionType = MenuItemOptionType.Extra, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PriceDelta = 120, IsRemovable = false, IsDefault = false, IsAvailable = true },
            new MenuItemOption { Id = 5, OptionGroupId = 3, Title = "Buffet setup", OptionType = MenuItemOptionType.Extra, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PriceDelta = 0, IsRemovable = false, IsDefault = true, IsAvailable = true },
            new MenuItemOption { Id = 6, OptionGroupId = 3, Title = "Plated service", OptionType = MenuItemOptionType.Extra, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PriceDelta = 180, IsRemovable = false, IsDefault = false, IsAvailable = true });

        modelBuilder.Entity<AppUser>().HasData(
            new AppUser { Id = 1, FullName = "CaterEase Admin", Email = "admin@caterease.local", PasswordHash = "PBKDF2$100000$AAAAAAAAAAAAAAAAAAAAAA==$iMGgUv1EvOOAQO682i83Gg2X3pL2BniTeSFy0dFj/Vs=", Role = AppRole.Admin, IsActive = true, IsTwoFactorEnabled = false, CreatedUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new AppUser { Id = 2, FullName = "Istanbul Garden Manager", Email = "caterer@caterease.local", PasswordHash = "PBKDF2$100000$AAAAAAAAAAAAAAAAAAAAAA==$iMGgUv1EvOOAQO682i83Gg2X3pL2BniTeSFy0dFj/Vs=", Role = AppRole.Caterer, CatererProfileId = 1, IsActive = true, IsTwoFactorEnabled = false, CreatedUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new AppUser { Id = 3, FullName = "Demo User", Email = "user@caterease.local", PasswordHash = "PBKDF2$100000$AAAAAAAAAAAAAAAAAAAAAA==$iMGgUv1EvOOAQO682i83Gg2X3pL2BniTeSFy0dFj/Vs=", Role = AppRole.User, CustomerProfileId = 1, IsActive = true, IsTwoFactorEnabled = false, CreatedUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
    }
}
