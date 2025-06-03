using Backend.Domain.Collections;
using Backend.Domain.Errors;
using FluentAssertions;

namespace Tests.Domain.Collections;

public class FollowedCollectionTests
{
    private readonly Guid _userId1 = Guid.NewGuid();
    private readonly Guid _userId2 = Guid.NewGuid();

    [Fact]
    public void Add_NewUser_AddsToFollowed()
    {
        // Arrange
        var collection = new FollowedCollection(new[] { _userId1 });
        
        // Act
        var result = collection.Add(_userId2);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.FollowedUsers.Should().BeEquivalentTo([_userId1, _userId2]);
    }

    [Fact]
    public void Add_ExistingUser_ReturnsFailure()
    {
        // Arrange
        var collection = new FollowedCollection(new[] { _userId1 });
        
        // Act
        var result = collection.Add(_userId1);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Already followed.");
    }

    [Fact]
    public void Remove_ExistingUser_RemovesFromFollowed()
    {
        // Arrange
        var collection = new FollowedCollection(new[] { _userId1, _userId2 });
        
        // Act
        var result = collection.Remove(_userId1);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.FollowedUsers.Should().BeEquivalentTo([_userId2]);
    }

    [Fact]
    public void Remove_NonExistentUser_ReturnsFailure()
    {
        // Arrange
        var collection = new FollowedCollection(new[] { _userId1 });
        
        // Act
        var result = collection.Remove(_userId2);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Not in your followed users.");
    }
}