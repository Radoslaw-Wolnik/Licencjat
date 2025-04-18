// Simple DTO for query projections
using Backend.Domain.Common;

namespace Backend.Application.DTOs;

public record UserProjection(
    Guid Id,
    string Email,
    string Username,
    string LocationCity,
    string LocationCountry,
    float Reputation
);