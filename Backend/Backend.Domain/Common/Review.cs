using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record Review(Guid Id, Guid UserId, Guid BookId, int Rating, string? Comment)
{

    public static Result<Review> Create(Guid id, Guid userId, Guid bookId, int rating, string? comment)
    {
        var errors = new List<IError>();
        
        if (rating < 1 || rating > 10) errors.Add(DomainErrorFactory.Invalid("Review.Rating", "Rating must be between 1 and 10"));
        if (userId == Guid.Empty) errors.Add(DomainErrorFactory.NotFound("User", userId));
        if (bookId == Guid.Empty) errors.Add(DomainErrorFactory.NotFound("GeneralBook", bookId));
        if (comment?.Length > 500) errors.Add(DomainErrorFactory.Invalid("Review.Comment", "The comment was too long (max 500 characters)"));


        return errors.Count != 0
        ? Result.Fail<Review>(errors)
        : new Review(
            id,
            userId,
            bookId,
            rating,
            comment
        );
    }
}