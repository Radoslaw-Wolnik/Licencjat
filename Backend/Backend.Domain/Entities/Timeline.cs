// Backend.Domain/Entities/Timeline.cs
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class Timeline : Entity<Guid>
{
    public string Description { get; }
    public TimelineStatus Status { get; }
    public Guid UserId { get; }
    public Guid SwapId { get; }

    private Timeline(Guid id, string description, TimelineStatus status, Guid userId, Guid swapId)
    {
        Id = id;
        Description = description;
        Status = status;
        UserId = userId;
        SwapId = swapId;
    }

    public static Result<Timeline> Create(string description, TimelineStatus status, Guid userId, Guid swapId)
    {
        var errors = new List<IError>();
        
        if (string.IsNullOrWhiteSpace(description)) 
            errors.Add(TimelineErrors.EmptyDescription);
        if (!Enum.IsDefined(typeof(TimelineStatus), status)) 
            errors.Add(TimelineErrors.InvalidStatus);
        if (userId == Guid.Empty) 
            errors.Add(UserErrors.NotFound);
        if (swapId == Guid.Empty) 
            errors.Add(SwapErrors.NotFound);

        if (errors.Any()) return Result.Fail<Timeline>(errors);

        return new Timeline(
            Guid.NewGuid(),
            description.Trim(),
            status,
            userId,
            swapId
        );
    }
}