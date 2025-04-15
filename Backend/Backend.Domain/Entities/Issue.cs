// Backend.Domain/Entities/Issue.cs
using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class Issue : Entity<Guid>
{
    public string Description { get; }
    public Guid UserId { get; }
    public Guid SubSwapId { get; }

    private Issue(Guid id, string description, Guid userId, Guid subSwapId)
    {
        Id = id;
        Description = description;
        UserId = userId;
        SubSwapId = subSwapId;
    }

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

        if (errors.Any()) return Result.Fail<Issue>(errors);

        return new Issue(
            Guid.NewGuid(),
            description.Trim(),
            userId,
            subSwapId
        );
    }
}