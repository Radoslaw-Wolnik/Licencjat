using Backend.Domain.Enums;

namespace Backend.Application.ReadModels.UserBooks;

// this is how other users see the user book
public sealed record UserBookDetailsReadModel(
    Guid Id,
    string Title,
    string Author,

    string LanguageCode,
    int PageCount,
    string CoverPhotoUrl,
    BookState State,
    BookStatus Status
);