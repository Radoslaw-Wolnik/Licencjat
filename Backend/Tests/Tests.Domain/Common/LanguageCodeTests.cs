using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Tests.Domain.Helpers;

namespace Tests.Domain.Common;

public class LanguageCodeTests
{
    [Theory]
    [InlineData("en")]
    [InlineData("pl")]
    public void Create_WithValidCode_ReturnsLanguageCode(string code)
    {
        // Act
        var result = LanguageCode.Create(code);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be(code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithEmptyCode_ReturnsError(string code)
    {
        // Act
        var result = LanguageCode.Create(code);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("LanguageCode", "Language code cannot be empty");
    }

    [Theory]
    [InlineData("EN")] // Uppercase
    [InlineData("Pl")] // Mixed case
    public void Create_NormalizesCodeToLower(string code)
    {
        // Act
        var result = LanguageCode.Create(code);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be(code.ToLower());
    }

    [Theory]
    [InlineData("xx")] // Not in valid list
    [InlineData("zz")] // Not in valid list
    public void Create_WithUnrecognizedCode_ReturnsError(string code)
    {
        // Act
        var result = LanguageCode.Create(code);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("LanguageCode", "Unrecognized language code");
    }
}