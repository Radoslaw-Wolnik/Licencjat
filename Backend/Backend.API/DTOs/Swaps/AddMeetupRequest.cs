using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Swaps;

public sealed record AddMeetupRequest(
    [Range(-90, 90)] double Latitude,
    [Range(-180, 180)] double Longitude
);
