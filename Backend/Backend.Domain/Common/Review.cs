using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record Review(Guid Id, Guid UserId, Guid BookId, int Rating, string? Comment)
{

    public static Result<Review> Create(Guid id, Guid userId, Guid bookId, int rating, string? comment)
    {
        var errors = new List<IError>();
        
        if (rating < 1 || rating > 10) errors.Add(ReviewErrors.InvalidRating);
        if (userId == Guid.Empty) errors.Add(UserErrors.NotFound);
        if (bookId == Guid.Empty) errors.Add(BookErrors.NotFound);
        if (comment?.Length > 500) errors.Add(ReviewErrors.CommentTooLong);


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