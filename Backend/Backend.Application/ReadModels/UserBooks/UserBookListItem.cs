using Backend.Application.ReadModels.Common;
using Backend.Domain.Enums;

namespace Backend.Application.ReadModels.UserBooks;

public sealed record UserBookListItem (
    Guid Id,
    string Title,
    string Author,
    string CoverUrl,
    UserSmallReadModel User,
    BookState State   // for list of userbooks based on the GeneralBook
);
// from user
// full - userId, username, reputtion, city, country, swapcount, profile picture