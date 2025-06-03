using Backend.Domain.Collections;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentAssertions;

namespace Tests.Domain.Collections;

public class SocialMediaCollectionTests
{
    private readonly SocialMediaLink _link1;
    private readonly SocialMediaLink _link2;

    public SocialMediaCollectionTests()
    {
        _link1 = SocialMediaLink.Create(
            Guid.NewGuid(),
            SocialMediaPlatform.Messenger,
            "https://facebook.com/user1").Value;
        
        _link2 = SocialMediaLink.Create(
            Guid.NewGuid(),
            SocialMediaPlatform.Instagram,
            "https://instagram.com/user1").Value;
    }

    [Fact]
    public void Add_NewLink_AddsToCollection()
    {
        // Arrange
        var collection = new SocialMediaCollection(new[] { _link1 });
        
        // Act
        var result = collection.Add(_link2);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Links.Should().BeEquivalentTo([_link1, _link2]);
    }

    [Fact]
    public void Add_AtLimit_ReturnsFailure()
    {
        // Arrange
        var links = Enumerable.Range(0, 10)
            .Select(i => SocialMediaLink.Create(
                Guid.NewGuid(),
                SocialMediaPlatform.Instagram,
                $"https://instagram.com/user{i}").Value);
        
        var collection = new SocialMediaCollection(links);
        var newLink = SocialMediaLink.Create(
            Guid.NewGuid(),
            SocialMediaPlatform.Skype,
            "https://skype.com/user1").Value;
        
        // Act
        var result = collection.Add(newLink);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Linmit of 10 links reached");
    }

    [Fact]
    public void Add_DuplicatePlatform_ReturnsFailure()
    {
        // Arrange
        var duplicatePlatform = SocialMediaLink.Create(
            Guid.NewGuid(),
            _link1.Platform, // Same platform
            "https://facebook.com/user2").Value;
        
        var collection = new SocialMediaCollection(new[] { _link1 });
        
        // Act
        var result = collection.Add(duplicatePlatform);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Platform already used - duplicate");
    }

    [Fact]
    public void Update_ValidLink_UpdatesCollection()
    {
        // Arrange
        var updated = SocialMediaLink.Create(
            _link1.Id,
            SocialMediaPlatform.Instagram, // Changed platform
            "https://instagram.com/user1").Value;
        
        var collection = new SocialMediaCollection(new[] { _link1 });
        
        // Act
        var result = collection.Update(updated);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Links.Should().ContainSingle().Which.Should().BeEquivalentTo(updated);
    }

    [Fact]
    public void Update_WithDuplicatePlatform_ReturnsFailure()
    {
        // Arrange
        var updated = SocialMediaLink.Create(
            _link1.Id,
            _link2.Platform, // Duplicate platform
            "https://twitter.com/user2").Value;
        
        var collection = new SocialMediaCollection(new[] { _link1, _link2 });
        
        // Act
        var result = collection.Update(updated);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("platform already taken");
    }
}