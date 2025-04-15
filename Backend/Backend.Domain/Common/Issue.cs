// Backend.Domain/Entities/Issue.cs
using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed record Issue(string Description, Guid UserId, Guid SubSwapId)
{
    public static Result<Issue> Create(string description, Guid userId, Guid subSwapId)
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
            description.Trim(),
            userId,
            subSwapId
        );
    }
}