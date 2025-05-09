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
            errors.Add(DomainErrorFactory.Invalid("Issue", "Description empty"));
        if (description.Length > 1000) 
            errors.Add(DomainErrorFactory.Invalid("Issue", "Description too long (above 1000 characters)"));
        if (userId == Guid.Empty) 
            errors.Add(DomainErrorFactory.NotFound("User", userId));
        if (subSwapId == Guid.Empty) 
            errors.Add(DomainErrorFactory.NotFound("SubSwap", subSwapId));

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