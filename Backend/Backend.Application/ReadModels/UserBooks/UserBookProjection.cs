namespace Backend.Application.ReadModels.UserBooks;

public record UserBookProjection(
    Guid Id,
    Guid UserId,
    string Title,
    string Author,

    string UserName,
    float UserReputation,
    string? ProfilePictureUrl,

    string CoverPhoto
);