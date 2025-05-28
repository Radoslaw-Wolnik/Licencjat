using Backend.Domain.Enums;

namespace Backend.Application.ReadModels.UserBooks;

public sealed record BookmarkReadModel(
    Guid Id,
    int Page,
    BookmarkColours Colour,
    string? Description
);
