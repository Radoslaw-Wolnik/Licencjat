namespace Backend.API.DTOs.GeneralBooks.Responses;

public sealed record GeneralBookListItemResponse(
    Guid Id,
    string Title,
    string Author,
    string CoverUrl,
    double RatingAvg,
    string PrimaryGenre,
    DateOnly? PublicationDate
);