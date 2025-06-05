using AutoMapper;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.Users;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Extensions;
using Backend.Infrastructure.Mapping;
using FluentAssertions;

namespace Tests.Infrastructure.Mapping;

public class UserReadModelProfileTests
{
    private readonly IMapper _mapper;

    public UserReadModelProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserReadModelProfile>();
            // cfg.AddProfile<GeneralBookReadModelProfile>();
            // cfg.AddProfile<UserBookReadModelProfile>();
        });
        
        _mapper = config.CreateMapper();
        config.AssertConfigurationIsValid(); // validate mapping
    }

    [Fact]
    public void UserEntity_To_UserSmallReadModel_MapsBasicDetails()
    {
        // Arrange
        var entity = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "basicuser",
            ProfilePicture = "basic.jpg"
        };

        // Act
        var smallModel = _mapper.Map<UserSmallReadModel>(entity);
        // var smallModel = entity.ToUserSmallReadModel();

        // Assert
        smallModel.UserId.Should().Be(entity.Id);
        smallModel.Username.Should().Be("basicuser");
        smallModel.ProfilePictureUrl.Should().Be("basic.jpg");
        smallModel.UserReputation.Should().BeNull();
        smallModel.City.Should().BeNull();
        smallModel.Country.Should().BeNull();
        smallModel.SwapCount.Should().BeNull();
    }

    [Fact]
    public void UserEntity_To_UserSmallReadModel_MapsFullDetails_WhenIncludeDetailsSet()
    {
        // Arrange
        var entity = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "detaileduser",
            ProfilePicture = "detailed.jpg",
            City = "New York",
            Country = "US",
            Reputation = 4.7f,
            SubSwaps = new List<SubSwapEntity>
            {
                new() 
                { 
                    Swap = new SwapEntity { Status = SwapStatus.Ongoing } 
                },
                new() 
                { 
                    Swap = new SwapEntity { Status = SwapStatus.Completed } 
                },
                new() 
                { 
                    Swap = new SwapEntity { Status = SwapStatus.Requested } // Should be excluded
                }
            }
        };

        // Act
        // var smallModel = entity.ToUserSmallReadModel(includeDetails: true);
        var smallModel = _mapper.Map<UserSmallReadModel>(entity, opts => opts.Items["IncludeDetails"] = true);

        // Assert
        smallModel.UserId.Should().Be(entity.Id);
        smallModel.Username.Should().Be("detaileduser");
        smallModel.ProfilePictureUrl.Should().Be("detailed.jpg");
        smallModel.UserReputation.Should().Be(4.7f);
        smallModel.City.Should().Be("New York");
        smallModel.Country.Should().Be("US");
        smallModel.SwapCount.Should().Be(2); // Only Ongoing and Completed count
    }

    [Fact]
    public void UserEntity_To_UserProfileReadModel_MapsCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var entity = new UserEntity
        {
            Id = userId,
            UserName = "profileuser",
            Reputation = 4.2f,
            City = "London",
            Country = "GB",
            ProfilePicture = "profile.jpg",
            Bio = "Book enthusiast",
            SocialMediaLinks = new List<SocialMediaLinkEntity>
            {
                new() { Platform = SocialMediaPlatform.Instagram, Url = "https://instagram.com/profile" }
            },
            Wishlist = new List<UserWishlistEntity>
            {
                new() 
                { 
                    GeneralBook = new GeneralBookEntity 
                    { 
                        Id = Guid.NewGuid(),
                        Title = "Wishlist Book",
                        CoverPhoto = "wishlist-cover.jpg"
                    } 
                }
            },
            UserBooks = new List<UserBookEntity>
            {
                new() 
                { 
                    Status = BookStatus.Reading,
                    Book = new GeneralBookEntity 
                    { 
                        Id = Guid.NewGuid(),
                        Title = "Reading Book",
                        CoverPhoto = "reading-cover.jpg"
                    }
                },
                new() 
                { 
                    Status = BookStatus.Finished,
                    Book = new GeneralBookEntity 
                    { 
                        Id = Guid.NewGuid(),
                        Title = "Library Book",
                        CoverPhoto = "library-cover.jpg"
                    }
                }
            },
            SubSwaps = new List<SubSwapEntity>
            {
                new() { Swap = new SwapEntity { Status = SwapStatus.Ongoing } },
                new() { Swap = new SwapEntity { Status = SwapStatus.Completed } },
                new() { Swap = new SwapEntity { Status = SwapStatus.Requested } } // Excluded
            }
        };

        // Act
        var profileModel = _mapper.Map<UserProfileReadModel>(entity);

        // Assert
        profileModel.Id.Should().Be(userId);
        profileModel.UserName.Should().Be("profileuser");
        profileModel.Reputation.Should().Be(4.2f);
        profileModel.SwapCount.Should().Be(2);
        profileModel.City.Should().Be("London");
        profileModel.Country.Should().Be("GB");
        profileModel.ProfilePictureUrl.Should().Be("profile.jpg");
        profileModel.Bio.Should().Be("Book enthusiast");
        
        profileModel.SocialMedias.Should().HaveCount(1);
        profileModel.SocialMedias.First().Platform.Should().Be(SocialMediaPlatform.Instagram);
        
        profileModel.Wishlist.Should().HaveCount(1);
        profileModel.Wishlist.First().Title.Should().Be("Wishlist Book");
        
        profileModel.Reading.Should().HaveCount(1);
        profileModel.Reading.First().Title.Should().Be("Reading Book");
        
        profileModel.UserLibrary.Should().HaveCount(2);
        profileModel.UserLibrary.First().Title.Should().Be("Reading Book");
    }

    [Fact]
    public void SocialMediaLinkEntity_To_SocialMediaLinkReadModel_MapsCorrectly()
    {
        // Arrange
        var entity = new SocialMediaLinkEntity
        {
            Platform = SocialMediaPlatform.WhatsApp,
            Url = "https://wa.me/user123"
        };

        // Act
        var linkModel = _mapper.Map<SocialMediaLinkReadModel>(entity);

        // Assert
        linkModel.Platform.Should().Be(SocialMediaPlatform.WhatsApp);
        linkModel.Url.Should().Be("https://wa.me/user123");
    }

    [Fact]
    public void UserProfileReadModel_HandlesEmptyCollections()
    {
        // Arrange
        var entity = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "emptyuser",
            SocialMediaLinks = new List<SocialMediaLinkEntity>(),
            Wishlist = new List<UserWishlistEntity>(),
            UserBooks = new List<UserBookEntity>()
        };

        // Act
        var profileModel = _mapper.Map<UserProfileReadModel>(entity);

        // Assert
        profileModel.SocialMedias.Should().BeEmpty();
        profileModel.Wishlist.Should().BeEmpty();
        profileModel.Reading.Should().BeEmpty();
        profileModel.UserLibrary.Should().BeEmpty();
    }

    [Fact]
    public void UserProfileReadModel_CalculatesSwapCountCorrectly()
    {
        // Arrange
        var entity = new UserEntity
        {
            SubSwaps = new List<SubSwapEntity>
            {
                new() { Swap = new SwapEntity { Status = SwapStatus.Ongoing } },
                new() { Swap = new SwapEntity { Status = SwapStatus.Completed } },
                new() { Swap = new SwapEntity { Status = SwapStatus.Completed } },
                new() { Swap = new SwapEntity { Status = SwapStatus.Disputed } }, // Excluded
                new() { Swap = new SwapEntity { Status = SwapStatus.Requested } } // Excluded
            }
        };

        // Act
        var profileModel = _mapper.Map<UserProfileReadModel>(entity);

        // Assert
        profileModel.SwapCount.Should().Be(3);
    }

    [Fact]
    public void UserSmallReadModel_HandlesNullProfilePicture()
    {
        // Arrange
        var entity = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "nophoto",
            ProfilePicture = null
        };

        // Act
        // var smallModel = entity.ToUserSmallReadModel();
        var smallModel = _mapper.Map<UserSmallReadModel>(entity);
        
        // Assert
        smallModel.ProfilePictureUrl.Should().BeNull();
    }

    [Fact]
    public void UserSmallReadModel_HandlesZeroSwaps()
    {
        // Arrange
        var entity = new UserEntity
        {
            SubSwaps = new List<SubSwapEntity>()
        };

        // Act
        var smallModel = entity.ToUserSmallReadModel(true);

        // Assert
        smallModel.SwapCount.Should().Be(0);
    }
}