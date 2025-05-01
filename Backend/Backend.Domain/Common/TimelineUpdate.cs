using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record TimelineUpdate(Guid Id, Guid UserId, TimelineStatus Status, string Description)
{
    public static Result<TimelineUpdate> Create(Guid id, Guid userId, TimelineStatus status,  string description)
    {

        var errors = new List<IError>();
        
        if (description.Length > 100)
            errors.Add(new Error("Description too long"));

        if (!Enum.IsDefined(typeof(TimelineStatus), status))
            return Result.Fail(TimelineErrors.InvalidStatus);

        if (string.IsNullOrWhiteSpace(description)) 
            errors.Add(TimelineErrors.EmptyDescription);

        if (userId == Guid.Empty) 
            errors.Add(UserErrors.NotFound);

        return errors.Count != 0
            ? Result.Fail<TimelineUpdate>(errors) 
            : new TimelineUpdate(id, userId, status, description.Trim());
    }

}

    