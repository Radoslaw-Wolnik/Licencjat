using Backend.Domain.Enums;

namespace Backend.Application.ReadModels.GeneralBooks;

public record GeneralBookProjection(
    Guid Id,
    string Title,
    string Author,
    string Language,
    float? RatingAvg,
    BookGenre BookGenre,
    DateOnly PublicationDate,
    string CoverPhoto
);

// in user addional user