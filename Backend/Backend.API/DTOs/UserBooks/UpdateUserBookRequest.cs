using Backend.Domain.Enums;

namespace Backend.API.DTOs.UserBooks;

public sealed record UpdateUserBookRequest(
    BookStatus? Status,
    BookState? State
);