using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Tests.Domain.Helpers;

namespace Tests.Domain.Common;

public class FeedbackTests
{
    private readonly Guid _validId = Guid.NewGuid();
    private readonly Guid _validSubSwapId = Guid.NewGuid();
    private readonly Guid _validUserId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidParameters_ReturnsFeedback()
    {
        // Act
        var result = Feedback.Create(
            _validId, _validSubSwapId, _validUserId,
            stars: 3, 
            recommend: true,
            length: SwapLength.JustRight,
            condition: SwapConditionBook.Same,
            communication: SwapCommunication.Perfect
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            SubSwapId = _validSubSwapId,
            UserId = _validUserId,
            Stars = 3,
            Recommend = true
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Create_WithInvalidStars_ReturnsError(int stars)
    {
        // Act
        var result = Feedback.Create(
            _validId, _validSubSwapId, _validUserId,
            stars, 
            true,
            SwapLength.JustRight, // im missing t
            SwapConditionBook.Same,
            SwapCommunication.Perfect
        );

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("Feedback", "Stars must be between 1 and 5");
    }

    [Fact]
    public void Create_WithInvalidUserId_ReturnsError()
    {
        // Act
        var result = Feedback.Create(
            _validId, _validSubSwapId, Guid.Empty,
            stars: 3, 
            true,
            SwapLength.JustRight,
            SwapConditionBook.Same,
            SwapCommunication.Perfect
        );

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeNotFoundError("User", Guid.Empty);
    }

    [Fact]
    public void Create_WithInvalidSubSwapId_ReturnsError()
    {
        // Act
        var result = Feedback.Create(
            _validId, Guid.Empty, _validUserId,
            stars: 3, 
            true,
            SwapLength.JustRight,
            SwapConditionBook.Same,
            SwapCommunication.Perfect
        );

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeNotFoundError("SubSwap", Guid.Empty);
    }
}