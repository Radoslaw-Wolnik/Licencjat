using FluentResults;

namespace Backend.Domain.Common;

public sealed record LocationCoordinates(double Latitude, double Longitude)
{
    public double Latitude { get; private set; } = Latitude;
    public double Longitude { get; private set; } = Longitude;
    public static Result<LocationCoordinates> Create(double latitude, double longitude)
    {
        if (Math.Abs(latitude) > 90 || Math.Abs(longitude) > 180)
            return Result.Fail("Invalid coordinates");
            
        return new LocationCoordinates(latitude, longitude);
    }
}