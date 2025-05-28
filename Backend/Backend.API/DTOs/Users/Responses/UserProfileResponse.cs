namespace Backend.API.DTOs.Users.Responses;

public sealed record UserProfileResponse(
    Guid Id,
    string Username,
    string Email,
    string? City,
    string? CountryCode,
    string? Bio,
    string? ProfilePictureUrl
);
