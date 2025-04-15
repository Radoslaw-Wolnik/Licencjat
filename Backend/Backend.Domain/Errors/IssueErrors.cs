using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class IssueErrors
{
    public static DomainError EmptyDescription => new(
        "Issue.EmptyDescription",
        "Issue description cannot be empty",
        ErrorType.BadRequest);

    public static DomainError DescriptionTooLong => new(
        "Issue.DescriptionTooLong",
        "Issue description cannot exceed 1000 characters",
        ErrorType.BadRequest);
}