// Domain/Errors/SocialMediaErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class BlockedErrors
{
    public static DomainError AlreadyBlocked => new(
        "Blocked.AlreadyBlocked",
        "You already blocked this user",
        ErrorType.Conflict);
    
    public static DomainError NotFound => new(
        "Blocked.NotFound",
        "The user youo want to block doesnt exsisit",
        ErrorType.NotFound);
    
    public static DomainError CannotBlockYourself => new(
        "Blocked.CannotBlockYourself",
        "You cannot block yourself !!! ",
        ErrorType.Conflict);
    
}
