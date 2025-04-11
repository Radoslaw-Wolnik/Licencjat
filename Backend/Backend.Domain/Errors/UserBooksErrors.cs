// Domain/Errors/UserBookErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class UserBookErrors
{
    public static DomainError NotFound => new(
        "UserBook.NotFound",
        "User book entry not found",
        ErrorType.NotFound);

    public static DomainError InvalidCondition => new(
        "UserBook.InvalidCondition",
        "Invalid book condition specified",
        ErrorType.Validation);

    public static DomainError UnavailableForSwap => new(
        "UserBook.Unavailable",
        "Book is currently unavailable for swapping",
        ErrorType.Conflict);
}
