using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Tests.Domain.Helpers;

namespace Tests.Domain.Common;

public class BioStringTests
{
    [Theory]
    [InlineData("Valid bio")]
    [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
    public void Create_WithValidValue_ReturnsBioString(string value)
    {
        // Act
        var result = BioString.Create(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(value);
    }

    [Fact]
    public void Create_WithLongValue_ReturnsError()
    {
        // Arrange
        var longBio = "A".Repeat(301);

        // Act
        var result = BioString.Create(longBio);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("Bio", "Bio must be max 300 characters");
    }

    [Fact]
    public void Initial_ReturnsEmptyBioString()
    {
        // Act
        var bio = BioString.Initial();

        // Assert
        bio.Value.Should().BeEmpty();
    }
}