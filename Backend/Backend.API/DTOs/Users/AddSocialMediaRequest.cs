using System.ComponentModel.DataAnnotations;
using Backend.Domain.Enums;

namespace Backend.API.DTOs.Users;

public sealed record AddSocialMediaRequest(
    [Required] SocialMediaPlatform Platform,
    [Required][Url] string Url
);
