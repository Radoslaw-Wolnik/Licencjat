using AutoMapper;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Mapping;
using FluentAssertions;
using FluentResults;

namespace Tests.Infrastructure.Mapping;

public class UserProfileTests
{
    private readonly IMapper _mapper;

    public UserProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
            cfg.AddProfile<SocialMediaProfile>();
            cfg.AddProfile<UserBookProfile>();
        });
        
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void UserEntity_To_User_MapsCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var entity = new UserEntity
        {
            Id = userId,
            Email = "user@example.com",
            UserName = "username123",
            FirstName = "John",
            LastName = "Doe",
            BirthDate = new DateOnly(1990, 1, 1),
            City = "New York",
            Country = "US",
            ProfilePicture = "https://example.com/avatar.jpg",
            Bio = "Software developer",
            Reputation = 4.5f,
            SocialMediaLinks = new List<SocialMediaLinkEntity>
            {
                new() 
                { 
                    Id = Guid.NewGuid(),
                    Platform = SocialMediaPlatform.Instagram,
                    Url = "https://instagram.com/johndoe"
                }
            },
            UserBooks = new List<UserBookEntity>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Language = "en",
                    PageCount = 300,
                    CoverPhoto = "cover1.jpg",
                    Status = BookStatus.Reading,
                    State = BookState.Available,
                    UserId = userId,
                    BookId = Guid.NewGuid()
                }
            },
            Wishlist = new List<UserWishlistEntity>
            {
                new() { GeneralBookId = Guid.NewGuid() }
            },
            Following = new List<UserFollowingEntity>
            {
                new() { FollowedId = Guid.NewGuid() }
            },
            Followers = new List<UserFollowingEntity>
            {
                new() { FollowerId = Guid.NewGuid() }
            },
            BlockedUsers = new List<UserBlockedEntity>
            {
                new() { BlockedId = Guid.NewGuid() }
            }
        };

        // Act
        var user = _mapper.Map<User>(entity);

        // Assert
        user.Id.Should().Be(entity.Id);
        user.Email.Should().Be(entity.Email);
        user.Username.Should().Be(entity.UserName);
        user.FirstName.Should().Be(entity.FirstName);
        user.LastName.Should().Be(entity.LastName);
        user.BirthDate.Should().Be(entity.BirthDate);
        user.Location.City.Should().Be("New York");
        user.Location.Country.Code.Should().Be("US");
        user.ProfilePicture?.Link.Should().Be("https://example.com/avatar.jpg");
        user.Bio.Value.Should().Be("Software developer");
        user.Reputation.Value.Should().Be(4.5f);
        
        user.SocialMediaLinks.Should().HaveCount(1);
        user.SocialMediaLinks.First().Platform.Should().Be(SocialMediaPlatform.Instagram);
        
        user.OwnedBooks.Should().HaveCount(1);
        user.OwnedBooks.First().OwnerId.Should().Be(userId);
        
        user.Wishlist.Should().HaveCount(1);
        user.Followed.Should().HaveCount(1);
        user.Followers.Should().HaveCount(1);
        user.Blocked.Should().HaveCount(1);
    }

    [Fact]
    public void User_To_UserEntity_MapsCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = User.Reconstitute(
            id: userId,
            email: "jane@example.com",
            username: "jane_doe",
            firstName: "Jane",
            lastName: "Doe",
            birthDate: new DateOnly(1985, 5, 15),
            location: new Location("London", new CountryCode("GB")),
            reputation: new Reputation(4.8f),
            profilePicture: new Photo("https://example.com/jane.jpg"),
            bio: new BioString("Book lover"),
            wishlist: new List<Guid> { Guid.NewGuid() },
            following: new List<Guid> { Guid.NewGuid() },
            followers: new List<Guid> { Guid.NewGuid() },
            blockedUsers: new List<Guid> { Guid.NewGuid() },
            ownedBooks: new List<UserBook>
            {
                UserBook.Reconstitute(
                    id: Guid.NewGuid(),
                    ownerId: userId,
                    generalBookId: Guid.NewGuid(),
                    status: BookStatus.Finished,
                    state: BookState.Borrowed,
                    language: LanguageCode.Create("pl").Value,
                    pageCount: 400,
                    coverPhoto: new Photo("cover2.jpg"),
                    bookmarks: Enumerable.Empty<Bookmark>()
                ).Value
            },
            socialMediaLinks: new List<SocialMediaLink>
            {
                new SocialMediaLink(
                    Id: Guid.NewGuid(),
                    Platform: SocialMediaPlatform.WhatsApp,
                    Url: "https://wa.me/janedoe"
                )
            }
        );

        // Act
        var entity = _mapper.Map<UserEntity>(user);

        // Assert
        entity.Id.Should().Be(userId);
        entity.Email.Should().Be("jane@example.com");
        entity.UserName.Should().Be("jane_doe");
        entity.FirstName.Should().Be("Jane");
        entity.LastName.Should().Be("Doe");
        entity.BirthDate.Should().Be(new DateOnly(1985, 5, 15));
        entity.City.Should().Be("London");
        entity.Country.Should().Be("GB");
        entity.ProfilePicture.Should().Be("https://example.com/jane.jpg");
        entity.Bio.Should().Be("Book lover");
        entity.Reputation.Should().Be(4.8f);
        
        // Collections should be ignored
        entity.SocialMediaLinks.Should().BeEmpty();
        entity.UserBooks.Should().BeEmpty();
        entity.Wishlist.Should().BeEmpty();
        entity.Following.Should().BeEmpty();
        entity.Followers.Should().BeEmpty();
        entity.BlockedUsers.Should().BeEmpty();
    }

    [Fact]
    public void UserEntity_To_User_HandlesNullValues()
    {
        // Arrange
        var entity = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User",
            BirthDate = new DateOnly(2000, 1, 1),
            City = "Paris",
            Country = "FR",
            ProfilePicture = null,
            Bio = null,
            Reputation = 3.0f
        };

        // Act
        var user = _mapper.Map<User>(entity);

        // Assert
        user.ProfilePicture.Should().BeNull();
        user.Bio.Value.Should().BeEmpty();
        user.SocialMediaLinks.Should().BeEmpty();
        user.OwnedBooks.Should().BeEmpty();
        user.Wishlist.Should().BeEmpty();
        user.Followed.Should().BeEmpty();
        user.Followers.Should().BeEmpty();
        user.Blocked.Should().BeEmpty();
    }

    [Fact]
    public void User_To_UserEntity_HandlesNullValues()
    {
        // Arrange
        var user = User.Reconstitute(
            id: Guid.NewGuid(),
            email: "nulls@example.com",
            username: "nulluser",
            firstName: "Null",
            lastName: "User",
            birthDate: new DateOnly(1995, 5, 5),
            location: new Location("Berlin", new CountryCode("DE")),
            reputation: new Reputation(3.5f),
            profilePicture: null,
            bio: new BioString(""),
            wishlist: Enumerable.Empty<Guid>(),
            following: Enumerable.Empty<Guid>(),
            followers: Enumerable.Empty<Guid>(),
            blockedUsers: Enumerable.Empty<Guid>(),
            ownedBooks: Enumerable.Empty<UserBook>(),
            socialMediaLinks: Enumerable.Empty<SocialMediaLink>()
        );

        // Act
        var entity = _mapper.Map<UserEntity>(user);

        // Assert
        entity.ProfilePicture.Should().BeNull();
        entity.Bio.Should().Be("");
    }

    [Fact]
    public void UserEntity_To_User_ThrowsForInvalidCountryCode()
    {
        // Arrange
        var entity = new UserEntity
        {
            Country = "INVALID", // Not a valid country code
            City = "Test City"
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<User>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*Bad country code*");
    }

    [Fact]
    public void UserEntity_To_User_ThrowsForInvalidReputation()
    {
        // Arrange
        var entity = new UserEntity
        {
            Reputation = 6.0f, // Invalid, max 5.0
            City = "Test City",
            Country = "US"
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<User>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*Bad reputation*");
    }

    [Fact]
    public void UserEntity_To_User_ThrowsForInvalidBio()
    {
        // Arrange
        var longBio = new string('x', 1001);
        var entity = new UserEntity
        {
            Bio = longBio,
            City = "Test City",
            Country = "GB"
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<User>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*Bad bio*");
    }

    [Fact]
    public void User_To_UserEntity_IgnoresCollections()
    {
        // Arrange
        var user = User.Reconstitute(
            id: Guid.NewGuid(),
            email: "collections@example.com",
            username: "collectionuser",
            firstName: "Collection",
            lastName: "User",
            birthDate: new DateOnly(1990, 1, 1),
            location: new Location("Tokyo", new CountryCode("JP")),
            reputation: new Reputation(4.0f),
            profilePicture: new Photo("avatar.jpg"),
            bio: new BioString("Test bio"),
            wishlist: new List<Guid> { Guid.NewGuid() },
            following: new List<Guid> { Guid.NewGuid() },
            followers: new List<Guid> { Guid.NewGuid() },
            blockedUsers: new List<Guid> { Guid.NewGuid() },
            ownedBooks: new List<UserBook>
            {
                UserBook.Reconstitute(
                    id: Guid.NewGuid(),
                    ownerId: Guid.NewGuid(),
                    generalBookId: Guid.NewGuid(),
                    status: BookStatus.Waiting,
                    state: BookState.Available,
                    language: LanguageCode.Create("ja").Value,
                    pageCount: 500,
                    coverPhoto: new Photo("cover.jpg"),
                    bookmarks: Enumerable.Empty<Bookmark>()
                ).Value
            },
            socialMediaLinks: new List<SocialMediaLink>
            {
                new SocialMediaLink(
                    Id: Guid.NewGuid(),
                    Platform: SocialMediaPlatform.WeeChat,
                    Url: "https://weechat.com/user"
                )
            }
        );

        // Act
        var entity = _mapper.Map<UserEntity>(user);

        // Assert
        entity.SocialMediaLinks.Should().BeEmpty();
        entity.UserBooks.Should().BeEmpty();
        entity.Wishlist.Should().BeEmpty();
        entity.Following.Should().BeEmpty();
        entity.Followers.Should().BeEmpty();
        entity.BlockedUsers.Should().BeEmpty();
    }
}
