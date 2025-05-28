namespace Backend.Application.ReadModels.Common;

// Shared in Common namespace
public sealed record UserSmallReadModel(
    Guid UserId,
    string Username,
    string? ProfilePictureUrl,
    float? UserReputation = null, // Optional
    string? City = null,          // Optional
    string? Country = null,       // Optional
    int? SwapCount = null         // Optional
);

// GeneralBooks/Swaps: Omit reputation/city (UserReputation: null)
// UserBooks: Include all fields