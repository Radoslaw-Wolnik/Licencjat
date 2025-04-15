// Backend.Domain/Common/SocialMediaLink.cs
using FluentResults;
namespace Backend.Domain.Common;

public sealed record SocialMediaLink
{
    public string Platform { get; }
    public string Url { get; }

    private SocialMediaLink(string platform, string url)
    {
        Platform = platform;
        Url = url;
    }

    public static Result<SocialMediaLink> Create(string platform, string url)
    {
        if (string.IsNullOrWhiteSpace(platform))
            return Result.Fail("Platform is required");
        
        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            return Result.Fail("Invalid URL format");

        return new SocialMediaLink(platform, url);
    }
}