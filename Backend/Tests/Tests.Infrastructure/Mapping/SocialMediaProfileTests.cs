using AutoMapper;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Mapping;
using FluentAssertions;
using FluentResults;

namespace Tests.Infrastructure.Mapping;

public class SocialMediaProfileTests
{
    private readonly IMapper _mapper;

    public SocialMediaProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<SocialMediaProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void SocialMediaLinkEntity_To_SocialMediaLink_MapsCorrectly()
    {
        // Arrange
        var entity = new SocialMediaLinkEntity
        {
            Id = Guid.NewGuid(),
            Platform = SocialMediaPlatform.Instagram,
            Url = "https://instagram.com/user123"
        };

        // Act
        var link = _mapper.Map<SocialMediaLink>(entity);

        // Assert
        link.Id.Should().Be(entity.Id);
        link.Platform.Should().Be(entity.Platform);
        link.Url.Should().Be(entity.Url);
    }

    [Fact]
    public void SocialMediaLink_To_SocialMediaLinkEntity_MapsCorrectly()
    {
        // Arrange
        var link = new SocialMediaLink(
            Id: Guid.NewGuid(),
            Platform: SocialMediaPlatform.WhatsApp,
            Url: "https://wa.me/123456789"
        );

        // Act
        var entity = _mapper.Map<SocialMediaLinkEntity>(link);

        // Assert
        entity.Id.Should().Be(link.Id);
        entity.Platform.Should().Be(link.Platform);
        entity.Url.Should().Be(link.Url);
    }

    [Fact]
    public void SocialMediaLinkEntity_To_SocialMediaLink_ThrowsForInvalidPlatform()
    {
        // Arrange
        var entity = new SocialMediaLinkEntity
        {
            Platform = (SocialMediaPlatform)999, // Invalid
            Url = "https://valid.com"
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<SocialMediaLink>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*Platform not supported*");
    }

    [Fact]
    public void SocialMediaLinkEntity_To_SocialMediaLink_ThrowsForInvalidUrl()
    {
        // Arrange
        var entity = new SocialMediaLinkEntity
        {
            Platform = SocialMediaPlatform.Messenger,
            Url = "invalid-url" // Not absolute URI
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<SocialMediaLink>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*Must be in url format*");
    }

    [Fact]
    public void SocialMediaLink_To_SocialMediaLinkEntity_IgnoresNavigationProperties()
    {
        // Arrange
        var link = new SocialMediaLink(
            Id: Guid.NewGuid(),
            Platform: SocialMediaPlatform.Skype,
            Url: "https://skype.com/user123"
        );

        // Act
        var entity = _mapper.Map<SocialMediaLinkEntity>(link);

        // Assert
        entity.User.Should().BeNull();
        entity.UserId.Should().Be(Guid.Empty);
    }
}