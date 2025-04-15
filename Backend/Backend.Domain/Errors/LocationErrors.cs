// Domain/Errors/LocationErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class LocationErrors
{
    public static DomainError WrongLocation => new(
        "UserErrors.WrongLocation",
        "The user location format is invalid",
        ErrorType.Validation);
}