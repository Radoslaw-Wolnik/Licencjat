using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Tests.Domain.Helpers;

namespace Tests.Domain.Common;

public class ReputationTests
{
    [Theory]
    [InlineData(1.0f)]
    [InlineData(3.0f)]
    [InlineData(5.0f)]
    public void Create_WithValidValue_ReturnsReputation(float value)
    {
        // Act
        var result = Reputation.Create(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0.9f)]
    [InlineData(5.1f)]
    public void Create_WithInvalidValue_ReturnsError(float value)
    {
        // Act
        var result = Reputation.Create(value);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("Reputation", "Reputation must be between 1 and 5");
    }

    [Fact]
    public void Initial_ReturnsDefaultReputation()
    {
        // Act
        var reputation = Reputation.Initial();

        // Assert
        reputation.Value.Should().Be(4.0f);
    }
}