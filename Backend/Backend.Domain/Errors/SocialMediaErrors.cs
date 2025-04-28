// Domain/Errors/SocialMediaErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class SocialMediaErrors
{
    public static DomainError ItemExists => new(
        "SocialMedia.Exists",
        "Item already exists in Social Media Links",
        ErrorType.Conflict);
    
    public static DomainError UrlAlreadyExists => new(
        "SocialMedia.UrlAlreadyExists",
        "the url was already used for an other Social Media Link",
        ErrorType.Conflict);

    public static DomainError PlatformAlreadyExists => new(
        "SocialMedia.PlatformAlreadyExists",
        "the platform was already used for an other Social Media Link",
        ErrorType.Conflict);
    
    public static DomainError NotFound => new(
        "SocialMedia.NotFound",
        "Item not found in user Social Media Links",
        ErrorType.NotFound);

    public static DomainError LimitReached => new(
        "SocialMedia.Limit",
        "Social Media Links ammount limit reached",
        ErrorType.Validation);
}
