using Backend.Application.ReadModels.Common;
namespace Backend.Application.ReadModels.GeneralBooks;

public sealed record ReviewReadModel(
    Guid Id,
    UserSmallReadModel User,
    // Guid BookId, its  not needed here
    int Rating,
    string? Comment,
    DateTime CreatedAt
);
// needed here from user: 
// UserId, Username, ProfilePicture