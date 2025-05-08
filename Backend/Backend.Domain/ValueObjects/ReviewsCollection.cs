using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.ValueObjects;

public class ReviewsCollection
{
    private readonly List<Review> _reviews = new();
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    public Result Add(Review review)
    {
        if (_reviews.Contains(review))
            return Result.Fail("Already added this review.");

        if (_reviews.Any(r => r.UserId == review.UserId))
            return Result.Fail("One person can only add one review");
        
        _reviews.Add(review);
        return Result.Ok();
    }

    public Result Remove(Guid reviewId)
    {
        var existing = _reviews.SingleOrDefault(r => r.Id == reviewId);
        if (existing == null)
            return Result.Fail("not found");
        
        _reviews.Remove(existing);
        return Result.Ok();
    }

    public Result Update(Review updatedReview){
        var oldReview = _reviews.SingleOrDefault(r => r.Id == updatedReview.Id);
        if (oldReview == null)
            return Result.Fail("cannot update review that doesnt exsisit");

        // logic
        if (oldReview.UserId != updatedReview.UserId)
            return Result.Fail("cannot update the user that made the review");
        
        // replace
        _reviews.Remove(oldReview);
        _reviews.Add(updatedReview);
        return Result.Ok();
    }
}
