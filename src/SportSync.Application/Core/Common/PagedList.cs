namespace SportSync.Application.Core.Common;

public class PagedList<T>
{
    private int _firstPageSize;

    public PagedList(IEnumerable<T> items, int page, int pageSize, int totalCount, int? firstPageSize = null)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        Items = items.ToList();
        _firstPageSize = firstPageSize ?? pageSize;
    }

    public int Page { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public bool HasNextPage => ((Page - 1) * PageSize) + _firstPageSize < TotalCount;

    public bool HasPreviousPage => Page > 1;

    public IReadOnlyCollection<T> Items { get; }
}