using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Tests.Domain.Helpers;

namespace Tests.Domain.Common;

public class MeetupTests
{
    private readonly Guid _validId = Guid.NewGuid();
    private readonly Guid _validSwapId = Guid.NewGuid();
    private readonly Guid _validSuggestedUserId = Guid.NewGuid();
    private readonly LocationCoordinates _validLocation = LocationCoordinates.Create(52.2297, 21.0122).Value;

    [Fact]
    public void Create_WithValidParameters_ReturnsMeetup()
    {
        // Act
        var result = Meetup.Create(
            _validId, _validSwapId, _validSuggestedUserId, 
            MeetupStatus.Proposed, _validLocation);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            SwapId = _validSwapId,
            SuggestedUserId = _validSuggestedUserId,
            Status = MeetupStatus.Proposed,
            Location = _validLocation
        });
    }

    [Fact]
    public void Create_WithInvalidUserId_ReturnsError()
    {
        // Act
        var result = Meetup.Create(
            _validId, _validSwapId, Guid.Empty,
            MeetupStatus.Proposed, _validLocation);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeNotFoundError("User", Guid.Empty);
        
    }

    [Fact]
    public void Create_WithInvalidStatus_ReturnsError()
    {
        // Act
        var result = Meetup.Create(
            _validId, _validSwapId, _validSuggestedUserId,
            (MeetupStatus)100, _validLocation);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("Meetup", "Invalid satus");
    }
}