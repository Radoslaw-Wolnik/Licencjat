using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class ReviewErrors
{
    public static DomainError InvalidRating => new(
        "Review.InvalidRating",
        "Rating must be between 1 and 10",
        ErrorType.BadRequest);

    public static DomainError CommentTooLong => new(
        "Review.CommentTooLong",
        "Review comment cannot exceed 500 characters",
        ErrorType.BadRequest);
}