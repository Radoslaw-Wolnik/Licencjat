using Backend.API.DTOs.GeneralBooks.Responses;

namespace Backend.API.DTOs.UserBooks.Responses;

public sealed record UserOwnBookProfileResponse(
    Guid Id,
    string Title,
    string Author,
    string LanguageCode,
    int PageCount,
    string CoverPhotoUrl,
    string State,
    string Status,
    ReviewResponse? UserReview,
    IEnumerable<SwapUserBookListItemResponse> Swaps,
    IEnumerable<BookmarkResponse> Bookmarks
);

public sealed record SwapUserBookListItemResponse(
    Guid Id,
    string Username,
    DateOnly CreatedAt,
    string Status,
    string CoverPhotoUrl
);