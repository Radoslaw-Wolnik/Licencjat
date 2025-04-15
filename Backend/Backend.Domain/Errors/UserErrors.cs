// Domain/Errors/UserErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class UserErrors
{
    public static DomainError EmailAlreadyExists => new(
        "User.EmailExists", 
        "Email is already registered",
        ErrorType.Conflict);

    public static DomainError UsernameTaken => new(
        "User.UsernameTaken",
        "Username is already taken",
        ErrorType.Conflict);

    public static DomainError Underage => new(
        "User.Underage",
        "User must be at least 13 years old",
        ErrorType.Validation);

    /*
    public static DomainError InvalidPassword => new(
        "User.InvalidPassword",
        "Password does not meet complexity requirements",
        ErrorType.Validation);
    */

    public static DomainError InvalidCredentials => new(
        "User.InvalidCredentials",
        "Invalid username or password",
        ErrorType.Unauthorized);

    public static DomainError AccountLocked => new(
        "User.AccountLocked",
        "Account is temporarily locked",
        ErrorType.Forbidden);
    

    // Existing authentication errors
    public static DomainError NotFound => new(
        "User.NotFound",
        "User not found",
        ErrorType.NotFound);

    public static DomainError ProfileUnavailable => new(
        "User.ProfileUnavailable",
        "Profile not available due to privacy settings",
        ErrorType.Forbidden);

    public static DomainError MaxBookLimit => new(
        "User.MaxBooks",
        "Maximum book limit reached",
        ErrorType.Validation);
    
    public static DomainError BookOvnershipMismatch => new(
        "UserErrors.BookOwnershipMismatch",
        "The userBook is not owned by current user",
        ErrorType.Conflict);
    
    public static DomainError WrongLocation => new(
        "UserErrors.WrongLocation",
        "The user location format is invalid",
        ErrorType.Validation);
}
