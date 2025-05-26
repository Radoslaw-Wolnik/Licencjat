namespace Backend.Domain.Common;

public record PaginatedResult<T>(
    List<T> Items,
    int TotalCount
);