using Backend.Domain.Enums;

namespace Backend.API.DTOs.UserBooks.Responses;

public sealed record BookmarkResponse(
    Guid Id,
    Guid UserBookId,
    BookmarkColours Colour,
    int Page,
    string? Description
);