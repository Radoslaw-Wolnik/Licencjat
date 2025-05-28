using Backend.Domain.Enums;

namespace Backend.API.DTOs.Swaps.Responses;

public sealed record MeetupResponse(
    Guid Id,
    Guid SwapId,
    double Latitude,
    double Longitude,
    MeetupStatus Status
);