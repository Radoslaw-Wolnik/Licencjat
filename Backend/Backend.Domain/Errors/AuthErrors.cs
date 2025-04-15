// Domain/Errors/AuthErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class AuthErrors
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

}