// Backend.Domain/Entities/Review.cs
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class Review : Entity<Guid>
{
    public int Rating { get; }
    public string? Comment { get; }
    public Guid UserId { get; }
    public Guid BookId { get; }

    private Review(Guid id, int rating, string? comment, Guid userId, Guid bookId)
    {
        Id = id;
        Rating = rating;
        Comment = comment;
        UserId = userId;
        BookId = bookId;
    }

    public static Result<Review> Create(int rating, string? comment, Guid userId, Guid bookId)
    {
        var errors = new List<IError>();
        
        if (rating < 1 || rating > 10) errors.Add(ReviewErrors.InvalidRating);
        if (userId == Guid.Empty) errors.Add(UserErrors.NotFound);
        if (bookId == Guid.Empty) errors.Add(BookErrors.NotFound);
        if (comment?.Length > 500) errors.Add(ReviewErrors.CommentTooLong);

        if (errors.Any()) return Result.Fail<Review>(errors);

        return new Review(
            Guid.NewGuid(),
            rating,
            comment,
            userId,
            bookId
        );
    }
}