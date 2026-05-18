namespace Northwind.Mvc.ViewModels;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; }
    public string? Search { get; set; }

    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)Math.Max(PageSize, 1));
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
