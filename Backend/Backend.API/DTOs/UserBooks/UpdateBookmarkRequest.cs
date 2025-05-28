using Backend.Domain.Enums;

namespace Backend.API.DTOs.UserBooks;

public sealed record UpdateBookmarkRequest(
    BookmarkColours? Colour,
    int? Page,
    string? Description
);