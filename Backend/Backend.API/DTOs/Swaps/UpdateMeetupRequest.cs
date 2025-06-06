using System.ComponentModel.DataAnnotations;
using Backend.Domain.Enums;

namespace Backend.API.DTOs.Swaps;

public sealed record UpdateMeetupRequest(
    [Required] Guid MeetupId,
    MeetupStatus Status,
    double? Latitude,
    double? Longitude
);