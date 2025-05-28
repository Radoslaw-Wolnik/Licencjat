using Backend.Domain.Enums;

namespace Backend.Application.ReadModels.Swaps;

public sealed record MeetupReadModel(
    Guid Id,
    Guid SwapId,
    double Latitude,
    double Longitude,
    DateTime ScheduledTime,
    MeetupStatus Status
);