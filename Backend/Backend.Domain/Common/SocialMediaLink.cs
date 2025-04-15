// Backend.Domain/Common/SocialMediaLink.cs
using FluentResults;
namespace Backend.Domain.Common;

public sealed record SocialMediaLink(string platform, string url)
{
    public string Platform { get; } = platform;
    public string Url { get; } = url;


    public static Result<SocialMediaLink> Create(string platform, string url)
    {
        if (string.IsNullOrWhiteSpace(platform))
            return Result.Fail("Platform is required");
        
        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            return Result.Fail("Invalid URL format");

        return new SocialMediaLink(platform, url);
    }
}