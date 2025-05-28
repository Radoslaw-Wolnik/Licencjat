namespace Backend.API.DTOs.Common;

public sealed record PaginatedResponse<T>(
    List<T> Items,
    int TotalCount,
    int Offset,
    int Limit
);