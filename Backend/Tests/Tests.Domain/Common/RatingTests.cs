using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Tests.Domain.Helpers;

namespace Tests.Domain.Common;

public class RatingTests
{
    [Theory]
    [InlineData(1.0f)]
    [InlineData(5.5f)]
    [InlineData(10.0f)]
    public void Create_WithValidValue_ReturnsRating(float value)
    {
        // Act
        var result = Rating.Create(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0.9f)]
    [InlineData(10.1f)]
    public void Create_WithInvalidValue_ReturnsError(float value)
    {
        // Act
        var result = Rating.Create(value);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("Rating", "Rating must be between 1 and 10");
    }

    [Fact]
    public void Initial_ReturnsDefaultRating()
    {
        // Act
        var rating = Rating.Initial();

        // Assert
        rating.Value.Should().Be(8.0f);
    }
}