namespace Backend.API.DTOs.Swaps.Responses;

public sealed record SubSwapResponse(
    string Title,
    string CoverPhotoUrl,
    int PageCount,
    string UserName,
    string? ProfilePictureUrl
);