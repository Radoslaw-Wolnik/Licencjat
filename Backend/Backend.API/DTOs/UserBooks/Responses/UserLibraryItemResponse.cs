namespace Backend.API.DTOs.UserBooks.Responses;

public sealed record UserLibraryItemResponse(
    Guid Id,
    string Title,
    string Author,
    string CoverUrl,
    string State,
    string Status,
    double RatingAvg
);