using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record Issue(Guid Id, Guid UserId, Guid SubSwapId, string Description)
{
    public static Result<Issue> Create(Guid id, Guid userId, Guid subSwapId, string description)
    {
        var errors = new List<IError>();
        
        if (string.IsNullOrWhiteSpace(description)) 
            errors.Add(IssueErrors.EmptyDescription);
        if (description.Length > 1000) 
            errors.Add(IssueErrors.DescriptionTooLong);
        if (userId == Guid.Empty) 
            errors.Add(UserErrors.NotFound);
        if (subSwapId == Guid.Empty) 
            errors.Add(SwapErrors.NotFound);

        return errors.Count != 0 
        ? Result.Fail<Issue>(errors)
        : new Issue(
            id,
            userId,
            subSwapId,
            description.Trim()
        );
    }
}