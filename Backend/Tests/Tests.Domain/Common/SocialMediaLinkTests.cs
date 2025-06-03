using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Tests.Domain.Helpers;

namespace Tests.Domain.Common;

public class SocialMediaLinkTests
{
    private readonly Guid _validId = Guid.NewGuid();

    [Theory]
    [InlineData(SocialMediaPlatform.Messenger, "https://facebook.com/user")]
    [InlineData(SocialMediaPlatform.Instagram, "https://instagram.com/user")]
    public void Create_WithValidParameters_ReturnsLink(SocialMediaPlatform platform, string url)
    {
        // Act
        var result = SocialMediaLink.Create(_validId, platform, url);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Platform = platform,
            Url = url
        });
    }

    [Fact]
    public void Create_WithInvalidPlatform_ReturnsError()
    {
        // Act
        var result = SocialMediaLink.Create(
            _validId, (SocialMediaPlatform)100, "https://valid.com");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("SocialMediaLink", "Platform not supported");
    }

    [Fact]
    public void Create_WithInvalidUrl_ReturnsError()
    {
        // Act
        var result = SocialMediaLink.Create(
            _validId, SocialMediaPlatform.Messenger, "invalid-url");

        // Assert        
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("SocialMediaLink", "Must be in url format");
    }
}