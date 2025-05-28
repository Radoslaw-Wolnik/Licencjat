using Backend.Domain.Enums;

namespace Backend.API.DTOs.GeneralBooks;

public sealed record UpdateGeneralBookRequest(
    string? Title,
    string? Author,
    DateOnly? Published,
    string? OriginalLanguage,
    IEnumerable<BookGenre>? NewBookGenres
);