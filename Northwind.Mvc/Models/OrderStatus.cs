namespace Northwind.Mvc.Models;

public static class OrderStatus
{
    public const string Pending = "Pending";
    public const string Paid = "Paid";
    public const string Preparing = "Preparing";
    public const string Ready = "Ready";
    public const string Delivered = "Delivered";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";

    public static readonly string[] All = [Pending, Paid, Preparing, Ready, Delivered, Completed, Cancelled];
    public static readonly string[] Rateable = [Paid, Preparing, Ready, Delivered, Completed];

    public static bool CanRate(string? status) =>
        Rateable.Contains(status);
}
