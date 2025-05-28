namespace Backend.API.DTOs.Swaps.Responses;

public sealed record TimelineUpdateResponse(
    string Comment,
    DateTime CreatedAt,
    string UserName,
    string? ProfilePictureUrl
);