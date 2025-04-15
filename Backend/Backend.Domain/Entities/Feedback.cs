// Backend.Domain/Entities/Feedback.cs
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class Feedback : Entity<Guid>
{
    public int Stars { get; }
    public bool Recommend { get; }
    public SwapLength Length { get; }
    public SwapConditionBook Condition { get; }
    public SwapCommunication Communication { get; }
    public Guid UserId { get; }
    public Guid SubSwapId { get; }

    private Feedback(Guid id, int stars, bool recommend, SwapLength length,
        SwapConditionBook condition, SwapCommunication communication, Guid userId, Guid subSwapId)
    {
        Id = id;
        Stars = stars;
        Recommend = recommend;
        Length = length;
        Condition = condition;
        Communication = communication;
        UserId = userId;
        SubSwapId = subSwapId;
    }

    public static Result<Feedback> Create(int stars, bool recommend, SwapLength length,
        SwapConditionBook condition, SwapCommunication communication, Guid userId, Guid subSwapId)
    {
        var errors = new List<IError>();
        
        if (stars < 1 || stars > 5) errors.Add(FeedbackErrors.InvalidStars);
        if (userId == Guid.Empty) errors.Add(UserErrors.NotFound);
        if (subSwapId == Guid.Empty) errors.Add(SwapErrors.NotFound);

        if (errors.Any()) return Result.Fail<Feedback>(errors);

        return new Feedback(
            Guid.NewGuid(),
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