using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Tests.Domain.Helpers;

namespace Tests.Domain.Common;

public class ReviewTests
{
    private readonly Guid _validId = Guid.NewGuid();
    private readonly Guid _validUserId = Guid.NewGuid();
    private readonly Guid _validBookId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidParameters_ReturnsReview()
    {
        // Act
        var result = Review.Create(
            _validId, _validUserId, _validBookId, 
            rating: 8, 
            createdAt: DateTime.UtcNow, 
            comment: "Great book!");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            UserId = _validUserId,
            BookId = _validBookId,
            Rating = 8,
            Comment = "Great book!"
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Create_WithInvalidRating_ReturnsError(int rating)
    {
        // Act
        var result = Review.Create(
            _validId, _validUserId, _validBookId,
            rating, DateTime.UtcNow, null);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("Review", "Rating must be between 1 and 10");
    }

    [Fact]
    public void Create_WithLongComment_ReturnsError()
    {
        // Arrange
        var longComment = new string('a', 501);

        // Act
        var result = Review.Create(
            _validId, _validUserId, _validBookId,
            5, DateTime.UtcNow, longComment);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("Review", "The comment was too long (max 500 characters)");
    }
}