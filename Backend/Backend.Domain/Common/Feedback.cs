using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record Feedback(
    Guid Id,
    Guid SubSwapId,
    Guid UserId, 
    int Stars, 
    bool Recommend, 
    SwapLength Length,
    SwapConditionBook Condition, 
    SwapCommunication Communication 
) {
    public static Result<Feedback> Create(
        Guid id,
        Guid subSwapId,
        Guid userId, 
        int stars, 
        bool recommend, 
        SwapLength length,
        SwapConditionBook condition, 
        SwapCommunication communication
    ) {
        var errors = new List<IError>();
        
        if (stars < 1 || stars > 5) errors.Add(DomainErrorFactory.Invalid("Feedback", "Stars must be between 1 and 5"));
        if (userId == Guid.Empty) errors.Add(DomainErrorFactory.NotFound("User", userId));
        if (subSwapId == Guid.Empty) errors.Add(DomainErrorFactory.NotFound("SubSwap", subSwapId));

        return errors.Count != 0
        ? Result.Fail<Feedback>(errors)
        : new Feedback(
            id,
            subSwapId,
            userId,
            stars,
            recommend,
            length,
            condition,
            communication            
        );
    }
}