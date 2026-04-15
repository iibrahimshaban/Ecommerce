namespace Ecommerce.Core.Pagination;
public class PaginatedList<T>(List<T> Items, int PageNumber, int count, double PageSize)
{
    public List<T> Items { get; private set; } = Items;
    public int PageNumber { get; private set; } = PageNumber;
    public int TotalPages { get; private set; } = (int)Math.Ceiling(count / PageSize);
    public int TotalCount { get; private set; } = count;
    public double PageSize { get; private set; } = PageSize;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static PaginatedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
