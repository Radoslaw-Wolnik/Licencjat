using Backend.Domain.Enums;

namespace Backend.API.DTOs.Swaps.Responses;

public sealed record SwapResponse(
    Guid Id,
    Guid UserRequestingId,
    Guid UserAcceptingId,
    Guid RequestedBookId,
    SwapStatus Status,
    DateTime CreatedAt
);
