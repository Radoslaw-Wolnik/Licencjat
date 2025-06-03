using Backend.Domain.Collections;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using FluentAssertions;

namespace Tests.Domain.Collections;

public class MeetupsCollectionTests
{
    private readonly Meetup _meetup1;
    private readonly Meetup _meetup2;

    public MeetupsCollectionTests()
    {
        _meetup1 = Meetup.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            MeetupStatus.Proposed,
            new LocationCoordinates(10, 20)).Value;
        
        _meetup2 = Meetup.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            MeetupStatus.Confirmed,
            new LocationCoordinates(30, 40)).Value;
    }

    [Fact]
    public void Add_WhenBelowLimit_AddsToCollection()
    {
        // Arrange
        var collection = new MeetupsCollection(Enumerable.Repeat(_meetup1, 9)); // removes duplicates
        
        // Act
        var result = collection.Add(_meetup2);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Meetups.Should().HaveCount(2);
    }

    [Fact]
    public void Add_WhenAtLimit_ReturnsFailure()
    {
        // Arrange
        var tenDistinct = Enumerable.Range(1, 10)
        .Select(i => Meetup.Create(
            Guid.NewGuid(),                  // a unique meetup.Id
            Guid.NewGuid(),                  // a unique swapId (doesn't really matter)
            Guid.NewGuid(),                  // a unique suggestedUserId
            MeetupStatus.Proposed,
            new LocationCoordinates(i, i)    // could be anything, just needs to succeed
        ).Value)
        .ToList();

        var collection = new MeetupsCollection(tenDistinct);

        // Act
        var result = collection.Add(_meetup2);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Max meetups count reached");
    }

    [Fact]
    public void Remove_ExistingMeetup_RemovesFromCollection()
    {
        // Arrange
        var collection = new MeetupsCollection(new[] { _meetup1, _meetup2 });
        
        // Act
        var result = collection.Remove(_meetup1.Id);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Meetups.Should().BeEquivalentTo([_meetup2]);
    }

    [Fact]
    public void Remove_NonExistentMeetup_ReturnsFailure()
    {
        // Arrange
        var collection = new MeetupsCollection(new[] { _meetup1 });
        
        // Act
        var result = collection.Remove(_meetup2.Id);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("not found");
    }

    [Fact]
    public void Update_ValidMeetup_UpdatesCollection()
    {
        // Arrange
        var updated = Meetup.Create(
            _meetup1.Id,
            _meetup1.SwapId,
            Guid.NewGuid(), // Different suggested user
            MeetupStatus.Confirmed,
            new LocationCoordinates(50, 60)).Value;
        
        var collection = new MeetupsCollection(new[] { _meetup1 });
        
        // Act
        var result = collection.Update(updated);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Meetups.Should().ContainSingle().Which.Should().BeEquivalentTo(updated);
    }
}