using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Tests.Domain.Helpers;

namespace Tests.Domain.Common;

public class LocationTests
{
    private readonly CountryCode _validCountry = CountryCode.FromCode("PL");

    [Fact]
    public void Create_WithValidParameters_ReturnsLocation()
    {
        // Arrange
        const string city = "Warsaw";

        // Act
        var result = Location.Create(city, _validCountry);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            City = city.Trim(),
            Country = _validCountry
        });
    }

    [Fact]
    public void Create_TrimsCityName()
    {
        // Arrange
        const string city = "  New York  ";

        // Act
        var result = Location.Create(city, _validCountry);

        // Assert
        result.Value.City.Should().Be("New York");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithEmptyCity_ReturnsError(string city)
    {
        // Act
        var result = Location.Create(city, _validCountry);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("Location", "City name empty");
    }

    [Fact]
    public void Create_WithLongCityName_ReturnsError()
    {
        // Arrange
        var longCity = new string('a', 101);

        // Act
        var result = Location.Create(longCity, _validCountry);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("Location", "City name too long");
    }

    [Fact]
    public void Deconstruct_ReturnsComponents()
    {
        // Arrange
        const string city = "Berlin";
        var location = Location.Create(city, _validCountry).Value;

        // Act
        var (deconstructedCity, deconstructedCountry) = location;

        // Assert
        deconstructedCity.Should().Be(city);
        deconstructedCountry.Should().Be(_validCountry);
    }
}