namespace Backend.API.DTOs.Common;

public sealed record UserSmallResponse(
    Guid Id,
    string Username,
    string? ProfilePictureUrl,
    float? UserReputation = null, // Optional
    string? City = null,          // Optional
    string? Country = null,       // Optional
    int? SwapCount = null         // Optional
);

// GeneralBooks/Swaps: Omit reputation/city (UserReputation: null)
// UserBooks: Include all fields