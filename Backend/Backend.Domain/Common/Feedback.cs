// Backend.Domain/Entities/Feedback.cs
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record Feedback(
    int Stars, 
    bool Recommend, 
    SwapLength Length,
    SwapConditionBook Condition, 
    SwapCommunication Communication, 
    Guid UserId, 
    Guid SubSwapId)
{
    public static Result<Feedback> Create(
        int stars, 
        bool recommend, 
        SwapLength length,
        SwapConditionBook condition, 
        SwapCommunication communication, 
        Guid userId, 
        Guid subSwapId)
    {
        var errors = new List<IError>();
        
        if (stars < 1 || stars > 5) errors.Add(FeedbackErrors.InvalidStars);
        if (userId == Guid.Empty) errors.Add(UserErrors.NotFound);
        if (subSwapId == Guid.Empty) errors.Add(SwapErrors.NotFound);

        return errors.Count != 0
        ? Result.Fail<Feedback>(errors)
        : new Feedback(
            stars,
            recommend,
            length,
            condition,
            communication,
            userId,
            subSwapId
        );
    }
}