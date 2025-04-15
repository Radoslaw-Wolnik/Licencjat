// Backend.Domain/Common/Location.cs
using FluentResults;

namespace Backend.Domain.Common;

public sealed record Location(string City, CountryCode Country)
{
    public static Result<Location> Create(string city, CountryCode country)
    {

        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(city))
            errors.Add(new Error("City cannot be empty"));
        
        if (city.Length > 100)
            errors.Add(new Error("City name too long"));

        return errors.Any() 
            ? Result.Fail<Location>(errors) 
            : new Location(city.Trim(), country);
    }
}