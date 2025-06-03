using Backend.Domain.Collections;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using FluentAssertions;

namespace Tests.Domain.Collections;

public class TimelineUpdatesCollectionTests
{
    private readonly TimelineUpdate _update1;
    private readonly TimelineUpdate _update2;

    public TimelineUpdatesCollectionTests()
    {
        _update1 = TimelineUpdate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            TimelineStatus.Requested,
            "Initial request").Value;
        
        _update2 = TimelineUpdate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            TimelineStatus.Finished,
            "Swap finished").Value;
    }

    [Fact]
    public void Add_FirstRequested_AddsToCollection()
    {
        // Arrange
        var collection = new TimelineUpdatesCollection(Enumerable.Empty<TimelineUpdate>());
        
        // Act
        var result = collection.Add(_update1);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.TimelineUpdates.Should().ContainSingle();
    }

    [Fact]
    public void Add_SecondRequested_ReturnsFailure()
    {
        // Arrange
        var collection = new TimelineUpdatesCollection(new[] { _update1 });
        var newRequest = TimelineUpdate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            _update1.SwapId,
            TimelineStatus.Requested,
            "Another request").Value;
        
        // Act
        var result = collection.Add(newRequest);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Can only request once during a timeline of a swap");
    }

    [Fact]
    public void Remove_ExistingUpdate_RemovesFromCollection()
    {
        // Arrange
        var collection = new TimelineUpdatesCollection(new[] { _update1, _update2 });
        
        // Act
        var result = collection.Remove(_update1.Id);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.TimelineUpdates.Should().BeEquivalentTo([_update2]);
    }
}