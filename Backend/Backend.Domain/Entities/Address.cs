// Domain/Entities/Address.cs
namespace Backend.Domain.Entities;

public class Address
{
    public Guid Id { get; private set; }
    public string Street { get; private set; }
    public string City { get; private set; }
    public Guid UserId { get; private set; }

    public Address(string street, string city)
    {
        Validate(street, city);
        Street = street;
        City = city;
    }

    private void Validate(string street, string city)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));
            
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
    }
}