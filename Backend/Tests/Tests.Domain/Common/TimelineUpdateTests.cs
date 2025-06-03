using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Tests.Domain.Helpers;

namespace Tests.Domain.Common;

public class TimelineUpdateTests
{
    private readonly Guid _validId = Guid.NewGuid();
    private readonly Guid _validUserId = Guid.NewGuid();
    private readonly Guid _validSwapId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidParameters_ReturnsTimelineUpdate()
    {
        // Arrange
        const string description = "Status update";

        // Act
        var result = TimelineUpdate.Create(
            _validId, _validUserId, _validSwapId, 
            TimelineStatus.Finished, description);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Description = description.Trim(),
            Status = TimelineStatus.Finished
        });
    }

    [Fact]
    public void Create_TrimsDescription()
    {
        // Arrange
        const string description = "  Update with spaces  ";

        // Act
        var result = TimelineUpdate.Create(
            _validId, _validUserId, _validSwapId, 
            TimelineStatus.Finished, description);

        // Assert
        result.Value.Description.Should().Be("Update with spaces");
    }

    [Fact]
    public void Create_WithLongDescription_ReturnsError()
    {
        // Arrange
        var longDescription = new string('a', 101);

        // Act
        var result = TimelineUpdate.Create(
            _validId, _validUserId, _validSwapId, 
            TimelineStatus.Finished, longDescription);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("TimelineUpdate", "Description too long");
    }

    [Fact]
    public void Create_WithEmptyDescription_ReturnsError()
    {
        // Act
        var result = TimelineUpdate.Create(
            _validId, _validUserId, _validSwapId,
            TimelineStatus.Finished, "  "); // Completed

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("TimelineUpdate", "Description of timelineUpdate was empty");
    }

    [Fact]
    public void Create_WithInvalidStatus_ReturnsError()
    {
        // Act
        var result = TimelineUpdate.Create(
            _validId, _validUserId, _validSwapId, 
            (TimelineStatus)100, "Valid description");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("TimelineUpdate", "Invalid status of the timalineUpdate");
    }
}