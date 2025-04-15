// Domain/Errors/UserErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class UserErrors
{

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
    
    public static DomainError BookOwnershipMismatch => new(
        "User.BookOwnershipMismatch",
        "User does not own this book",
        ErrorType.Conflict);

    public static DomainError MaxSocialMediaLinks => new(
        "User.MaxSocialMediaLinks",
        "Maximum of 10 social media links reached",
        ErrorType.Validation);

    public static DomainError AlreadyFollowing => new(
        "User.AlreadyFollowing",
        "Already following this user",
        ErrorType.Conflict);
}
