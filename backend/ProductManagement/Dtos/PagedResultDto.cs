namespace ProductManagement.Dtos;

public class PagedResultDto<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public PaginationMetadataDto Pagination { get; set; } = new();
}

public class PaginationMetadataDto
{
    public int CurrentPage { get; set; }
    public int PerPage { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}