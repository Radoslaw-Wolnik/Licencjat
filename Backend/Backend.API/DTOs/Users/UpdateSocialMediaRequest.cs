using System.ComponentModel.DataAnnotations;
using Backend.Domain.Enums;

namespace Backend.API.DTOs.Users;

public sealed record UpdateSocialMediaRequest(
    Guid SocialMediaId,
    SocialMediaPlatform? Platform,
    [Url] string? Url
);
