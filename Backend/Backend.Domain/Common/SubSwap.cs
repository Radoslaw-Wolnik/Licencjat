using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record SubSwap(
        Guid Id,
        Guid UserId, 
        int PageAt,
        UserBook? UserBookReading,
        Feedback? Feedback,
        Issue? Issue
        )
{
    public static Result<SubSwap> Create(
        Guid id,
        Guid userId, 
        int pageAt,
        UserBook? userBookReading,
        Feedback? feedback,
        Issue? issue
        )
    {
        var errors = new List<IError>();
        
        if (pageAt <= 0) errors.Add(DomainErrorFactory.Invalid("SubSwap", "Page must be above 0 or equal 0"));
        if (userId == Guid.Empty) errors.Add(DomainErrorFactory.NotFound("User", userId));

        return errors.Count != 0
        ? Result.Fail<SubSwap>(errors)
        : new SubSwap(
            id,
            userId,
            pageAt,
            userBookReading,
            feedback,
            issue
        );
    }

    public static SubSwap Initial(Guid userId, UserBook? userBookReading){
        return new SubSwap(Guid.NewGuid(), userId, 0, userBookReading, null, null);
    }
}