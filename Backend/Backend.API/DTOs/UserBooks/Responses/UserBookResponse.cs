using Backend.Domain.Enums;

namespace Backend.API.DTOs.UserBooks.Responses;

public sealed record UserBookResponse(
    Guid Id,
    Guid UserId,
    Guid BookId,
    BookStatus Status,
    BookState State,
    string Language,
    int PageCount,
    string CoverUrl
);
