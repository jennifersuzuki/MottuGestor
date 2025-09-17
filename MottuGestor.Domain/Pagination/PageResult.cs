namespace MottuGestor.Domain.Pagination;

public class PageResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    public bool HasMore { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public long Total { get; set; }
}