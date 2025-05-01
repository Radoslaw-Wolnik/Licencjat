using FluentResults;

namespace Backend.Domain.Common;

public sealed record Location
{
    public string City { get; }
    public CountryCode Country { get; }

    public Location(string city, CountryCode country)
    {
        City = city;
        Country = country;
    }

    public static Result<Location> Create(string city, CountryCode country)
    {

        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(city))
            errors.Add(new Error("City cannot be empty"));
        
        if (city.Length > 100)
            errors.Add(new Error("City name too long"));

        return errors.Count != 0
            ? Result.Fail<Location>(errors) 
            : new Location(city.Trim(), country);
    }
    
    public void Deconstruct(out string city, out CountryCode country)
    {
        city = City;
        country = Country;
    }

}