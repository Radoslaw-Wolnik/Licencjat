using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Users;

public sealed record UpdateProfileRequest(
    string? City,
    string? CountryCode,
    [MaxLength(500)] string? Bio
);
