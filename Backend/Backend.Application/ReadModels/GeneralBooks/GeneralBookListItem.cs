namespace Backend.Application.ReadModels.GeneralBooks;

public sealed record GeneralBookListItem (
    Guid Id,
    string Title,
    string Author,
    string CoverUrl,
    float RatingAvg
);