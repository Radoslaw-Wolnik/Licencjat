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
    
    public static DomainError InvalidState => new(
        "UserBook.InvalidState",
        "Book state is invalid",
        ErrorType.Validation);
    
    public static DomainError InvalidStatus => new(
        "UserBook.InvalidStatus",
        "Book status is invalid",
        ErrorType.Validation);

    public static DomainError InvalidPageCount => new(
        "UserBook.InvalidPageCount",
        "Number of pages must be higher then 0",
        ErrorType.Validation);
}
