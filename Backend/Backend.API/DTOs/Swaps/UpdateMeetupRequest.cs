using Backend.Domain.Enums;

namespace Backend.API.DTOs.Swaps;

public sealed record UpdateMeetupRequest(
    MeetupStatus Status,
    double? Latitude,
    double? Longitude
);