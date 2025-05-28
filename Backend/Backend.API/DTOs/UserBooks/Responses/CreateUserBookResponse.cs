namespace Backend.API.DTOs.UserBooks.Responses;

public sealed record CreateUserBookResponse(
    Guid UserBookId,
    string ImageKey
);