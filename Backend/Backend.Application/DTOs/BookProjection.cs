// Simple DTO for query projections
using Backend.Domain.Common;

namespace Backend.Application.DTOs;

public record BookProjection(
    Guid Id,
    string Title,
    string Author,
    string Language,
    float? ReviewAvg
);

// in user addional user