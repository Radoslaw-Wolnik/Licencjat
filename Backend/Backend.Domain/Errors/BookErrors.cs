// Domain/Errors/BookErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class BookErrors
{
    public static DomainError NotFound => new(
        "Book.NotFound",
        "Book not found in catalog",
        ErrorType.NotFound);

    public static DomainError InvalidMetadata => new(
        "Book.InvalidMetadata",
        "Invalid book metadata provided",
        ErrorType.Validation);
}
