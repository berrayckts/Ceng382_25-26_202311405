namespace Northwind.Mvc.Models;

public class ProjectDashboardViewModel
{
    public required string BrandName { get; init; }
    public required string HeroTitle { get; init; }
    public required string HeroDescription { get; init; }
    public required IReadOnlyList<ProjectNavItemViewModel> SidebarItems { get; init; }
    public required IReadOnlyList<ProjectCategoryViewModel> Categories { get; init; }
    public required IReadOnlyList<ProjectMenuCardViewModel> FeaturedItems { get; init; }
    public required IReadOnlyList<ProjectStatViewModel> Highlights { get; init; }
    public required IReadOnlyList<ProjectCartItemViewModel> CartItems { get; init; }
}

public class ProjectNavItemViewModel
{
    public required string Label { get; init; }
    public required string Icon { get; init; }
    public bool IsActive { get; init; }
}

public class ProjectCategoryViewModel
{
    public required string Icon { get; init; }
    public required string Name { get; init; }
}

public class ProjectMenuCardViewModel
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Price { get; init; }
    public required string AccentClass { get; init; }
}

public class ProjectStatViewModel
{
    public required string Title { get; init; }
    public required string Description { get; init; }
}

public class ProjectCartItemViewModel
{
    public required string Name { get; init; }
    public required string Price { get; init; }
}
