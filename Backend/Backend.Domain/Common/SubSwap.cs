using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record SubSwap(
        Guid Id,
        Guid UserId, 
        int PageAt,
        Guid? UserBookReadingId,
        Guid? FeedbackId,
        Guid? IssueId
        )
{
    public static Result<SubSwap> Create(
        Guid id,
        Guid userId, 
        int pageAt,
        Guid? userBookReadingId,
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
            id,
            userId,
            pageAt,
            userBookReadingId,
            feedbackId,
            issueId
        );
    }

    public static SubSwap Initial(Guid userId, Guid? userBookReadingId){
        return new SubSwap(Guid.NewGuid(), userId, 0, userBookReadingId, null, null);
    }
}