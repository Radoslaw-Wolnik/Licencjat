namespace Backend.API.DTOs.Common;

public sealed record BookCoverItemResponse(
    Guid Id,
    string Title,
    string CoverUrl
);