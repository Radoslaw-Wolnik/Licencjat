using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;
namespace Backend.Domain.Common;

public sealed record TimelineUpdate(string Description, TimelineStatus Status, Guid userId)
{
    public static Result<TimelineUpdate> Create(string description, TimelineStatus status, Guid userId)
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
            : new TimelineUpdate(description.Trim(), status, userId);
    }

}

    