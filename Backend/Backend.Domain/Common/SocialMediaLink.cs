using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record SocialMediaLink(Guid Id, SocialMediaPlatform Platform, string Url)
{
    public static Result<SocialMediaLink> Create(Guid id, SocialMediaPlatform platform, string url)
    {
        if (!Enum.IsDefined(typeof(SocialMediaPlatform), platform)) 
            return Result.Fail(DomainErrorFactory.Invalid("SocialMediaLink", "Platform not supported"));
        
        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            return Result.Fail(DomainErrorFactory.Invalid("SocialMediaLink", "Must be in url format"));

        return new SocialMediaLink(id, platform, url);
    }
}