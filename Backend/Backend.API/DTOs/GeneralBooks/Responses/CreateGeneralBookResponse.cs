namespace Backend.API.DTOs.GeneralBooks.Responses;

public sealed record CreateGeneralBookResponse(
    Guid BookId,
    string ImageKey
);