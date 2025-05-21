using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record TimelineUpdate(Guid Id, Guid UserId, Guid SwapId, TimelineStatus Status, string Description)
{
    public static Result<TimelineUpdate> Create(Guid id, Guid userId, Guid swapId, TimelineStatus status,  string description)
    {

        var errors = new List<IError>();
        
        if (description.Length > 100)
            errors.Add(new Error("Description too long"));

        if (!Enum.IsDefined(typeof(TimelineStatus), status))
            return Result.Fail(DomainErrorFactory.Invalid("Status", "Invalid status of the timalineUpdate"));

        if (string.IsNullOrWhiteSpace(description)) 
            errors.Add(DomainErrorFactory.Invalid("Description", "Description of timelineUpdate was empty"));

        if (userId == Guid.Empty) 
            errors.Add(DomainErrorFactory.NotFound("User", userId));

        return errors.Count != 0
            ? Result.Fail<TimelineUpdate>(errors) 
            : new TimelineUpdate(id, userId, swapId, status, description.Trim());
    }

}

    