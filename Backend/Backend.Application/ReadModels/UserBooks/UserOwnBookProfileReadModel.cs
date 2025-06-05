using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Enums;

namespace Backend.Application.ReadModels.UserBooks;

// this is how user sees their own user book
public sealed record UserOwnBookProfileReadModel(
    Guid Id,
    string Title,
    string Author,

    string LanguageCode,
    int PageCount,
    string CoverPhotoUrl,
    BookState State,
    BookStatus Status,

    ReviewReadModel? UserReview,
    IReadOnlyCollection<SwapUserBookListItem> Swaps,
    IReadOnlyCollection<BookmarkReadModel> Bookmarks // just first 10
    
);

public sealed record SwapUserBookListItem(
    Guid Id,
    string Username,
    DateOnly CreatedAt,
    SwapStatus Status,
    string CoverPhotoUrl
);