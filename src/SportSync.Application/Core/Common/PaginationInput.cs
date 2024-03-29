using SportSync.Application.Core.Constants;

namespace SportSync.Application.Core.Common;

public abstract class PaginationInput
{
    private int _firstPageSize = PaginationConstants.FirstPageSize;
    private int _pageSize;
    public int Page { get; set; }
    public int? FirstPageSize { get; set; }

    public int PageSize
    {
        get => Page == 1 ? FirstPageSize ?? _firstPageSize : _pageSize;
        set => _pageSize = value;
    }
}