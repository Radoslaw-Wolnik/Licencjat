// Backend.Domain/Common/SocialMediaLink.cs
using Backend.Domain.Enums;
using FluentResults;
namespace Backend.Domain.Common;

public sealed record SocialMediaLink(Guid Id, SocialMediaPlatform Platform, string Url)
{
    public Guid Id { get; private set; } = Id;
    public SocialMediaPlatform Platform { get; private set; } = Platform;
    public string Url { get; private set; } = Url;


    public static Result<SocialMediaLink> Create(Guid id, SocialMediaPlatform platform, string url)
    {
        if (Enum.IsDefined(typeof(SocialMediaPlatform), platform)) 
            return Result.Fail("Platform is invalid type - not supported");
        
        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            return Result.Fail("Invalid URL format");

        return new SocialMediaLink(id, platform, url);
    }
}