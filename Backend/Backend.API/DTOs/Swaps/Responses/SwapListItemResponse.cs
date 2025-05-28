using Backend.API.DTOs.Common;

namespace Backend.API.DTOs.Swaps.Responses;

public sealed record SwapListItemResponse(
    Guid Id,
    string MyBookCoverUrl,
    string TheirBookCoverUrl,
    UserSmallResponse User,
    string Status,
    DateTime CreatedAt
);
