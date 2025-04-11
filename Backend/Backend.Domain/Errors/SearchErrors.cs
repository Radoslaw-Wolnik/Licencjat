// Domain/Errors/SearchErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class SearchErrors
{
    public static DomainError InvalidCriteria => new(
        "Search.Invalid",
        "Invalid search criteria",
        ErrorType.Validation);

    public static DomainError Timeout => new(
        "Search.Timeout",
        "Search operation timed out",
        ErrorType.ServiceUnavailable);
}