namespace Backend.Application.ReadModels.GeneralBooks;

public sealed record ReviewReadModel(
    Guid Id,
    UserSmallReadModel User,
    // Guid BookId, its  not needed here
    int Rating,
    string? Comment
);

// instead of userId it would be better to actually populate review data 
// so that the frontend can build review view properly
public sealed record UserSmallReadModel(
    Guid UserId,
    string Username,
    string? ProfilePictureUrl
    // mby float userReputation but not sure - not needed here but needed in other places and we could then reuse smallUserDto across whole app
);
