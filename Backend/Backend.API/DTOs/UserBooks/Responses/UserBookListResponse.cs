using Backend.API.DTOs.Common;

namespace Backend.API.DTOs.UserBooks.Responses;

public sealed record UserBookListItemResponse(
    Guid Id,
    string Title,
    string Author,
    string CoverUrl,
    UserSmallResponse User,
    string State
);