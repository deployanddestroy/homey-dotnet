namespace Homey.Api.Common;

public interface IPagedRequest
{
    public const int MaxPageSize = 100;
    int? Page { get; }
    int? PageSize { get; }
}

public record PagedList<T>(List<T> Items, int Page, int PageSize, int TotalItemCount)
{
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page * PageSize < TotalItemCount;
}

/// <summary>
/// Extends the IQueryable to page the results
/// </summary>
public static class PaginationExtensions
{
    public static async Task<PagedList<TResponse>> ToPagedListAsync<TRequest, TResponse>(
        this IQueryable<TResponse> query, TRequest request, CancellationToken cancellationToken = default) where TRequest : IPagedRequest
    {
        // Get the page from the request, or default to 1
        var page = request.Page ?? 1;
        // Get the page size from the request, or default to 10
        var pageSize = request.PageSize ?? 10;
        
        // Verify the parameters are valid
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(page, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(pageSize, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, IPagedRequest.MaxPageSize);
        
        // Get the total number of items in the result
        var totalItemCount = await query.CountAsync(cancellationToken);
        // Skip and take, then turn the results into a List
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        // Return the result
        return new PagedList<TResponse>(items, page, pageSize, totalItemCount);
    }
}