using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentAssertions;

namespace Tests.Domain.Entities;

public class UserTests
{
    private const string Email = "test@example.com";
    private const string Username = "testuser";
    private const string FirstName = "Test";
    private const string LastName = "User";
    private readonly DateOnly _birthDate = new(1990, 1, 1);


    [Fact]
    public void Create_WithValidParameters_CreatesUser()
    {
        // Arrange
        CountryCode Country = CountryCode.Create("GB").ValueOrDefault;
        Location _location = Location.Create("London", Country).ValueOrDefault;

        // Act
        var result = User.Create(Email, Username, FirstName, LastName, _birthDate, _location);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Email,
            Username,
            FirstName,
            LastName,
            BirthDate = _birthDate,
            Location = _location
        });
    }

    [Fact]
    public void Create_UnderageUser_ReturnsError()
    {
        // Arrange
        CountryCode Country = CountryCode.Create("GB").ValueOrDefault;
        Location _location = Location.Create("London", Country).ValueOrDefault;
        var underageBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-12));
        
        // Act
        var result = User.Create(Email, Username, FirstName, LastName, underageBirthDate, _location);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("underage"));
    }

    [Fact]
    public void Reconstitute_CreatesUserWithAllProperties()
    {
        // Arrange
        CountryCode Country = CountryCode.Create("GB").ValueOrDefault;
        Location _location = Location.Create("London", Country).ValueOrDefault;

        var id = Guid.NewGuid();
        var reputation = new Reputation(4.5f);
        var profilePicture = new Photo("profile.jpg");
        var bio = new BioString("Test bio");
        var wishlist = new[] { Guid.NewGuid() };
        
        // Act
        var user = User.Reconstitute(
            id, Email, Username, FirstName, LastName, _birthDate, _location, 
            reputation, profilePicture, bio, wishlist, Enumerable.Empty<Guid>(), 
            Enumerable.Empty<Guid>(), Enumerable.Empty<Guid>(), 
            Enumerable.Empty<UserBook>(), Enumerable.Empty<SocialMediaLink>());

        // Assert
        user.Should().BeEquivalentTo(new
        {
            Id = id,
            Email,
            Username,
            FirstName,
            LastName,
            BirthDate = _birthDate,
            Location = _location,
            Reputation = reputation,
            ProfilePicture = profilePicture,
            Bio = bio,
            Wishlist = wishlist
        });
    }

    [Fact]
    public void UpdateProfilePicture_ChangesPhoto()
    {
        // Arrange
        CountryCode Country = CountryCode.Create("GB").ValueOrDefault;
        Location _location = Location.Create("London", Country).ValueOrDefault;

        var user = User.Create(Email, Username, FirstName, LastName, _birthDate, _location).Value;
        var newPhoto = new Photo("new-profile.jpg");
        
        // Act
        user.UpdateProfilePicture(newPhoto);
        
        // Assert
        user.ProfilePicture.Should().BeEquivalentTo(newPhoto);
    }

    [Fact]
    public void UpdateLocation_ChangesLocation()
    {
        // Arrange
        CountryCode Country = CountryCode.Create("GB").ValueOrDefault;
        Location _location = Location.Create("London", Country).ValueOrDefault;

        var user = User.Create(Email, Username, FirstName, LastName, _birthDate, _location).Value;
        Location newLocation = Location.Create("Manchaster", Country).ValueOrDefault;
        
        // Act
        user.UpdateLocation(newLocation);
        
        // Assert
        user.Location.Should().BeEquivalentTo(newLocation);
    }
}