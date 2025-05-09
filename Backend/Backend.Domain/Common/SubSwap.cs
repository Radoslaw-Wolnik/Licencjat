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
        
        if (pageAt < 0) errors.Add(DomainErrorFactory.Invalid("SubSwap", "Page must be above 0"));
        if (userId == Guid.Empty) errors.Add(DomainErrorFactory.NotFound("User", userId));
        if (userBookReadingId == Guid.Empty) errors.Add(DomainErrorFactory.NotFound("UserBook", userBookReadingId));

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