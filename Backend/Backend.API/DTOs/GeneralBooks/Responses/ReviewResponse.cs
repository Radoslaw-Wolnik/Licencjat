using Backend.API.DTOs.Common;

namespace Backend.API.DTOs.GeneralBooks.Responses;

public sealed record ReviewResponse(
    Guid Id,
    UserSmallResponse User,
    int Rating,
    string? Comment,
    DateTime CreatedAt
);
