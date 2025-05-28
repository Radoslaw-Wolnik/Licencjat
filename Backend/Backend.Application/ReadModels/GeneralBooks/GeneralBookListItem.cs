using Backend.Domain.Enums;

namespace Backend.Application.ReadModels.GeneralBooks;

public sealed record GeneralBookListItem (
    Guid Id,
    string Title,
    string Author,
    string CoverUrl,
    float RatingAvg,
    // for search
    BookGenre? PrimaryGenre = null, // Most lists need 1 genre
    DateOnly? PublicationDate = null
);