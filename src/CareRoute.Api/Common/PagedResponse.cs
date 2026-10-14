// src/CareRoute.Api/Common/PagedResponse.cs   (Api layer · shared HTTP contract)
namespace CareRoute.Api.Common;

public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)   // ①
{
    public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);    // ②

    public bool HasNextPage => Page < TotalPages;                                                        // ③
}