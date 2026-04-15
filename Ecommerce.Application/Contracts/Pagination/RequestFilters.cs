namespace Ecommerce.Application.Contracts.Pagination;
public record RequestFilters
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchName { get; init; }
    public string? SearchCategory { get; init; }
    public string? SortColumn { get; init; }
    public string SortDirection { get; init; } = "asc";
    public int MinPrice { get; init; } = 0;
    public int MaxPrice { get; init; } = int.MaxValue;
}