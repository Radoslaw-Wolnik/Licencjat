using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.GeneralBooks;

public sealed record ReviewRequest(
    [Range(1, 5)] int Rating,
    [MaxLength(1000)] string? Comment
);