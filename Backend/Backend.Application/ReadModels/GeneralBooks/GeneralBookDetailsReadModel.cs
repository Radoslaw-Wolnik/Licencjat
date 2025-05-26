using Backend.Domain.Enums;

namespace Backend.Application.ReadModels.GeneralBooks;

public sealed record GeneralBookDetailsReadModel (
    Guid Id,
    string Title,
    string Author,
    DateOnly Published,
    string LanguageCode,
    double RatingAvg,
    string CoverPhotoUrl,
    ICollection<BookGenre> Genres,
    IReadOnlyCollection<ReviewReadModel> Reviews // just first 10
);