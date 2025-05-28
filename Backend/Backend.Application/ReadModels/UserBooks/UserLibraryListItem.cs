using Backend.Domain.Enums;

namespace Backend.Application.ReadModels.UserBooks;

public sealed record UserLibraryListItem (
    Guid Id,
    string Title,
    string Author,
    string CoverUrl,
    BookState State,
    BookStatus Status,
    float RatingAvg // from general book
);
