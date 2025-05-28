using Backend.Domain.Enums;

namespace Backend.API.DTOs.Users.Responses;

public sealed record SocialMediaResponse(
    Guid Id,
    SocialMediaPlatform Platform,
    string Url
);