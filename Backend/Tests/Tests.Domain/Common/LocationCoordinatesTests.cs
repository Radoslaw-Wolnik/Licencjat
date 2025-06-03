using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Tests.Domain.Helpers;

namespace Tests.Domain.Common;

public class LocationCoordinatesTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(45, 90)]
    [InlineData(-45, -90)]
    public void Create_WithValidCoordinates_ReturnsCoordinates(double lat, double lon)
    {
        // Act
        var result = LocationCoordinates.Create(lat, lon);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Latitude = lat,
            Longitude = lon
        });
    }

    [Theory]
    [InlineData(91, 0)]
    [InlineData(-91, 0)]
    [InlineData(0, 181)]
    [InlineData(0, -181)]
    public void Create_WithInvalidCoordinates_ReturnsError(double lat, double lon)
    {
        // Act
        var result = LocationCoordinates.Create(lat, lon);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("LocationCoordinates", "Invalid coordinates");
    }
}