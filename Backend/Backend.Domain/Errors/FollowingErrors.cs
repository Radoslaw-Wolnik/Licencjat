// Domain/Errors/SocialMediaErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class FollowingErrors
{
    public static DomainError AlreadyFollowing => new(
        "Following.AlreadyFollowing",
        "You already follow this user",
        ErrorType.Conflict);
    
    public static DomainError NotFound => new(
        "Following.NotFound",
        "The user youo want to follow doesnt exsisit",
        ErrorType.NotFound);
    
    public static DomainError CannotFollowYourself => new(
        "Following.CannotFollowYourself",
        "You cannot follow yourself !!! ",
        ErrorType.Conflict);
    
}
