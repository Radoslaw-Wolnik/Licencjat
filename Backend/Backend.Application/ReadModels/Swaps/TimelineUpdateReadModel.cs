using Backend.Domain.Enums;

namespace Backend.Application.ReadModels.Swaps;

public sealed record TimelineUpdateReadModel(
    // TimelineStatus Status, // my without this one
    string Comment,
    DateTime CreatedAt,

    string UserName,
    string? ProfilePictureUrl
);