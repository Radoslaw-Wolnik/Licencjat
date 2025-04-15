// Backend.Domain/Entities/SubSwap.cs
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record SubSwap(
        Guid UserId, 
        Guid UserBookReadingId,
        int PageAt,
        Guid? FeedbackId,
        Guid? IssueId
        )
{
    public static Result<SubSwap> Create(
        Guid userId, 
        Guid userBookReadingId,
        int pageAt,
        Guid? feedbackId,
        Guid? issueId
        )
    {
        var errors = new List<IError>();
        
        if (pageAt < 0) errors.Add(SwapErrors.NegativePageNumber);
        if (userId == Guid.Empty) errors.Add(UserErrors.NotFound);
        if (userBookReadingId == Guid.Empty) errors.Add(BookErrors.NotFound);

        return errors.Count != 0
        ? Result.Fail<SubSwap>(errors)
        : new SubSwap(
            userId,
            userBookReadingId,
            pageAt,
            feedbackId,
            issueId
        );
    }
}