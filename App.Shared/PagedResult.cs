using System.Text.Json.Serialization;

namespace App.Shared;

public class PagedResult<T> : Result
{
    [JsonPropertyName("items")]
    public IEnumerable<T> Items { get; }

    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; }

    [JsonPropertyName("totalPages")]
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public PagedResult(bool isSuccess, IEnumerable<T> items, int count, int pageNumber, int pageSize, Error error) 
        : base(isSuccess, error)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static PagedResult<T> Success(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        return new PagedResult<T>(true, items, count, pageNumber, pageSize, Error.None);
    }

    public new static PagedResult<T> Failure(Error error)
    {
        return new PagedResult<T>(false, Enumerable.Empty<T>(), 0, 0, 0, error);
    }
    
    
}