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

    public static DomainError InvalidGenre => new(
        "Book.InvalidGenre",
        "Specified genre is not valid",
        ErrorType.BadRequest);

    public static DomainError DuplicateGenre => new(
        "Book.DuplicateGenre",
        "Book already contains this genre",
        ErrorType.Conflict);

    public static DomainError DuplicateCopy => new(
        "Book.DuplicateCopy",
        "User already has a copy of this book",
        ErrorType.Conflict);

    public static DomainError InvalidPublicationDate => new(
        "Book.InvalidPublicationDate",
        "Publication date cannot be in the future",
        ErrorType.BadRequest);
}
