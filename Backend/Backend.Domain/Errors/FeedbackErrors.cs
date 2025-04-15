using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class FeedbackErrors
{
    public static DomainError InvalidStars => new(
        "Feedback.InvalidStars",
        "Stars must be between 1 and 5",
        ErrorType.BadRequest);
}