namespace Northwind.Mvc.Models;

public static class AppRole
{
    public const string Admin = "Admin";
    public const string RestaurantOwner = "RestaurantOwner";
    public const string Customer = "Customer";
    public const string LegacyCaterer = "Caterer";
    public const string LegacyUser = "User";

    // Backward-compatible aliases for the existing controllers and older seeded rows.
    public const string Caterer = RestaurantOwner;
    public const string User = Customer;

    public static readonly string[] All = [Admin, RestaurantOwner, Customer];
    public static readonly string[] Legacy = [LegacyCaterer, LegacyUser];
    public const string CustomerOrAdmin = Customer + "," + Admin + "," + LegacyUser;
    public const string CustomerOnly = Customer + "," + LegacyUser;
    public const string RestaurantOwnerOnly = RestaurantOwner + "," + LegacyCaterer;
}
