using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Tests.Domain.Helpers;

namespace Tests.Domain.Common;

public class CountryCodeTests
{
    [Theory]
    [InlineData("US")]
    [InlineData("PL")]
    [InlineData("GB")]
    public void Create_WithValidCode_ReturnsCountryCode(string code)
    {
        // Act
        var result = CountryCode.Create(code);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be(code.ToUpper());
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithEmptyCode_ReturnsError(string code)
    {
        // Act
        var result = CountryCode.Create(code);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("CountryCode", "empty");
    }

    [Theory]
    [InlineData("USA")] // Valid format but not in list
    [InlineData("XX")]  // Not in valid list
    public void Create_WithUnrecognizedCode_ReturnsError(string code)
    {
        // Act
        var result = CountryCode.Create(code);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeNotFoundError("CountryCode", code.ToUpper());
    }

    [Theory]
    [InlineData("us")] // Lowercase
    [InlineData("Pl")] // Mixed case
    public void Create_NormalizesCodeToUpper(string code)
    {
        // Act
        var result = CountryCode.Create(code);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be(code.ToUpper());
    }

    [Fact]
    public void FromCode_CreatesInstance()
    {
        // Arrange
        const string code = "TEST";

        // Act
        var countryCode = CountryCode.FromCode(code);

        // Assert
        countryCode.Code.Should().Be(code);
    }
}