using AutoMapper;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.Swaps;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Mapping;
using FluentAssertions;

namespace Tests.Infrastructure.Mapping;

public class SwapReadModelProfileTests
{
    private readonly IMapper _mapper;

    public SwapReadModelProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SwapReadModelProfile>();
            cfg.AddProfile<UserReadModelProfile>();
        });
        
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void SwapEntity_To_SwapListItem_MapsCorrectly_ForRequestingUser()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        
        var swap = new SwapEntity
        {
            Id = Guid.NewGuid(),
            Status = SwapStatus.Ongoing,
            CreatedAt = new DateOnly(2023, 6, 1),
            SubSwapRequesting = new SubSwapEntity
            {
                UserId = currentUserId,
                UserBookReading = new UserBookEntity { CoverPhoto = "my-book.jpg" },
                User = new UserEntity { Id = currentUserId }
            },
            SubSwapAccepting = new SubSwapEntity
            {
                UserId = otherUserId,
                UserBookReading = new UserBookEntity { CoverPhoto = "their-book.jpg" },
                User = new UserEntity 
                { 
                    Id = otherUserId,
                    UserName = "otheruser",
                    ProfilePicture = "other-avatar.jpg"
                }
            }
        };

        // Act
        var listItem = _mapper.Map<SwapListItem>(swap, opts => 
            opts.Items["UserId"] = currentUserId);

        // Assert
        listItem.Id.Should().Be(swap.Id);
        listItem.MyBookCoverUrl.Should().Be("my-book.jpg");
        listItem.TheirBookCoverUrl.Should().Be("their-book.jpg");
        // listItem.Status.Should().Be(SwapStatus.Ongoing);
        listItem.CreatedAt.Should().Be(swap.CreatedAt);
        
        listItem.User.UserId.Should().Be(otherUserId);
        listItem.User.Username.Should().Be("otheruser");
        listItem.User.ProfilePictureUrl.Should().Be("other-avatar.jpg");
    }

    [Fact]
    public void SwapEntity_To_SwapListItem_MapsCorrectly_ForAcceptingUser()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        
        var swap = new SwapEntity
        {
            Id = Guid.NewGuid(),
            Status = SwapStatus.Requested,
            CreatedAt = new DateOnly(2023, 6, 2),
            SubSwapRequesting = new SubSwapEntity
            {
                UserId = otherUserId,
                UserBookReading = new UserBookEntity { CoverPhoto = "their-book.jpg" },
                User = new UserEntity 
                { 
                    Id = otherUserId,
                    UserName = "requester",
                    ProfilePicture = "requester-avatar.jpg"
                }
            },
            SubSwapAccepting = new SubSwapEntity
            {
                UserId = currentUserId,
                UserBookReading = new UserBookEntity { CoverPhoto = "my-book.jpg" },
                User = new UserEntity { Id = currentUserId }
            }
        };

        // Act
        var listItem = _mapper.Map<SwapListItem>(swap, opts => 
            opts.Items["UserId"] = currentUserId);

        // Assert
        listItem.MyBookCoverUrl.Should().Be("my-book.jpg");
        listItem.TheirBookCoverUrl.Should().Be("their-book.jpg");
        listItem.User.UserId.Should().Be(otherUserId);
        listItem.User.Username.Should().Be("requester");
    }

    [Fact]
    public void TimelineEntity_To_TimelineUpdateReadModel_MapsCorrectly()
    {
        // Arrange
        var entity = new TimelineEntity
        {
            Description = "Swap accepted",
            CreatedAt = new DateTime(2023, 6, 15, 10, 30, 0),
            User = new UserEntity
            {
                UserName = "timelineuser",
                ProfilePicture = "timeline-avatar.jpg"
            }
        };

        // Act
        var timelineModel = _mapper.Map<TimelineUpdateReadModel>(entity);

        // Assert
        timelineModel.Comment.Should().Be("Swap accepted");
        timelineModel.CreatedAt.Should().Be(new DateTime(2023, 6, 15, 10, 30, 0));
        timelineModel.UserName.Should().Be("timelineuser");
        timelineModel.ProfilePictureUrl.Should().Be("timeline-avatar.jpg");
    }

    [Fact]
    public void SwapEntity_To_SwapDetailsReadModel_MapsCorrectly_ForRequestingUser()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var maxUpdates = 5;
        
        var swap = new SwapEntity
        {
            Id = Guid.NewGuid(),
            CreatedAt = new DateOnly(2023, 6, 1),
            Status = SwapStatus.Ongoing,
            SubSwapRequesting = new SubSwapEntity
            {
                UserId = currentUserId,
                UserBookReading = new UserBookEntity
                {
                    CoverPhoto = "requesting-cover.jpg",
                    PageCount = 300,
                    Book = new GeneralBookEntity { Title = "Requesting Book" },
                    User = new UserEntity { UserName = "requester", ProfilePicture = "" }
                },
                User = new UserEntity { UserName = "requester", ProfilePicture = ""}
            },
            SubSwapAccepting = new SubSwapEntity
            {
                UserId = otherUserId,
                UserBookReading = new UserBookEntity
                {
                    CoverPhoto = "accepting-cover.jpg",
                    PageCount = 400,
                    Book = new GeneralBookEntity { Title = "Accepting Book" },
                    User = new UserEntity
                    {
                        UserName = "accepter",
                        SocialMediaLinks = new List<SocialMediaLinkEntity>
                        {
                            new() { Platform = SocialMediaPlatform.WhatsApp, Url = "wa.me/accepter" }
                        },
                        ProfilePicture = ""
                    }
                },
                User = new UserEntity
                    {
                        UserName = "accepter",
                        SocialMediaLinks = new List<SocialMediaLinkEntity>
                        {
                            new() { Platform = SocialMediaPlatform.WhatsApp, Url = "wa.me/accepter" }
                        },
                        ProfilePicture = ""
                    }
            },
            TimelineUpdates = new List<TimelineEntity>
            {
                new()
                {
                    Description = "Update 1",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    User = new UserEntity { UserName = "user1" },
                    
                    
                },
                new() 
                { 
                    Description = "Update 2", 
                    CreatedAt = DateTime.UtcNow,
                    User = new UserEntity { UserName = "user2" }
                }
            }
        };

        // Act
        var details = _mapper.Map<SwapDetailsReadModel>(swap, opts => 
        {
            opts.Items["CurrentUserId"] = currentUserId;
            opts.Items["MaxUpdates"] = maxUpdates;
        });

        // Assert
        details.Id.Should().Be(swap.Id);
        details.CreatedAt.Should().Be(swap.CreatedAt);
        // details.LastStatus.Should().Be(SwapStatus.Ongoing);
        
        details.MySubSwap.Title.Should().Be("Requesting Book");
        details.MySubSwap.CoverPhotoUrl.Should().Be("requesting-cover.jpg");
        details.MySubSwap.PageCount.Should().Be(300);
        details.MySubSwap.UserName.Should().Be("requester");
        
        details.TheirSubSwap.Should().NotBeNull();
        details.TheirSubSwap.Title.Should().Be("Accepting Book");
        
        details.SocialMediaLinks.Should().HaveCount(1);
        details.SocialMediaLinks.First().Url.Should().Be("wa.me/accepter");
        
        details.Updates.Should().HaveCount(2);
        details.Updates.First().Comment.Should().Be("Update 2"); // Ordered descending
    }

    [Fact]
    public void SwapEntity_To_SwapDetailsReadModel_LimitsUpdates()
    {
        // Arrange
        var swap = new SwapEntity
        {
            TimelineUpdates = Enumerable.Range(1, 10)
                .Select(i => new TimelineEntity 
                { 
                    Description = $"Update {i}",
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    User = new UserEntity()
                })
                .ToList(),
            SubSwapRequesting = new SubSwapEntity { UserId = Guid.NewGuid() },
            SubSwapAccepting = new SubSwapEntity { UserId = Guid.NewGuid() }
        };

        var maxUpdates = 3;

        // Act
        var details = _mapper.Map<SwapDetailsReadModel>(swap, opts => 
        {
            opts.Items["CurrentUserId"] = swap.SubSwapRequesting.UserId;
            opts.Items["MaxUpdates"] = maxUpdates;
        });

        // Assert
        details.Updates.Should().HaveCount(maxUpdates);
        details.Updates.Select(u => u.Comment)
            .Should().ContainInOrder("Update 1", "Update 2", "Update 3");
    }

    [Fact]
    public void SubSwapEntity_To_SubSwapReadModel_MapsCorrectly()
    {
        // Arrange
        var subSwap = new SubSwapEntity
        {
            UserBookReading = new UserBookEntity
            {
                CoverPhoto = "subswap-cover.jpg",
                PageCount = 250,
                Book = new GeneralBookEntity { Title = "SubSwap Book" },
                User = new UserEntity
                {
                    UserName = "subswapuser",
                    ProfilePicture = "subswap-avatar.jpg"
                }
            },
            User = new UserEntity
                {
                    UserName = "subswapuser",
                    ProfilePicture = "subswap-avatar.jpg"
                }
        };

        // Act
        var readModel = _mapper.Map<SubSwapReadModel>(subSwap);

        // Assert
        readModel.Title.Should().Be("SubSwap Book");
        readModel.CoverPhotoUrl.Should().Be("subswap-cover.jpg");
        readModel.PageCount.Should().Be(250);
        readModel.UserName.Should().Be("subswapuser");
        readModel.ProfilePictureUrl.Should().Be("subswap-avatar.jpg");
    }

    [Fact]
    public void SubSwapEntity_To_SubSwapReadModel_HandlesMissingBook()
    {
        // Arrange
        var subSwap = new SubSwapEntity
        {
            UserBookReading = null,
            User = new UserEntity { UserName = "nobookuser" }
        };

        // Act
        var readModel = _mapper.Map<SubSwapReadModel>(subSwap);

        // Assert
        readModel.Title.Should().BeNull();
        readModel.CoverPhotoUrl.Should().BeNull();
        readModel.PageCount.Should().Be(0);
        readModel.UserName.Should().Be("nobookuser");
    }

    [Fact]
    public void SwapDetailsReadModel_ShowsCorrectSocialLinks_ForAcceptingUser()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();
        
        var swap = new SwapEntity
        {
            SubSwapRequesting = new SubSwapEntity
            {
                UserId = requestingUserId,
                UserBookReading = new UserBookEntity
                {
                    CoverPhoto = "subswap-cover.jpg",
                    PageCount = 250,
                    Book = new GeneralBookEntity { Title = "SubSwap Book" },
                    User = new UserEntity
                    {
                        UserName = "subswapuser",
                        ProfilePicture = "subswap-avatar.jpg"
                    }
                },
                User = new UserEntity
                {
                    SocialMediaLinks = new List<SocialMediaLinkEntity>
                    {
                        new() { Platform = SocialMediaPlatform.Instagram, Url = "instagram.com/requester" }
                    }
                }
            },
            SubSwapAccepting = new SubSwapEntity
            {
                UserId = currentUserId,
                UserBookReading = new UserBookEntity
                {
                    CoverPhoto = "subswap-cover.jpg",
                    PageCount = 250,
                    Book = new GeneralBookEntity { Title = "SubSwap Book" },
                    User = new UserEntity
                    {
                        UserName = "subswapuser",
                        ProfilePicture = "subswap-avatar.jpg"
                    }
                },
                User = new UserEntity
                {
                    SocialMediaLinks = new List<SocialMediaLinkEntity>
                    {
                        new() { Platform = SocialMediaPlatform.WhatsApp, Url = "wa.me/accepter" }
                    }
                }
            }
        };

        // Act
        
        var details = _mapper.Map<SwapDetailsReadModel>(swap, opts => 
        {
            opts.Items["CurrentUserId"] = swap.SubSwapRequesting.UserId;
            opts.Items["MaxUpdates"] = 2;
        });

        // Assert
        details.SocialMediaLinks.Should().HaveCount(1);
        details.SocialMediaLinks.First().Url.Should().Be("wa.me/accepter");
    }

    [Fact]
    public void SwapListItem_HandlesMissingTheirBookCover()
    {
        // Arrange
        var swap = new SwapEntity
        {
            SubSwapRequesting = new SubSwapEntity
            {
                UserBookReading = new UserBookEntity { CoverPhoto = "my-cover.jpg" }
            },
            SubSwapAccepting = new SubSwapEntity
            {
                UserBookReading = null // Missing their book
            }
        };

        // Act
        var listItem = _mapper.Map<SwapListItem>(swap, opts => 
            opts.Items["UserId"] = swap.SubSwapRequesting.UserId);

        // Assert
        listItem.MyBookCoverUrl.Should().Be("my-cover.jpg");
        listItem.TheirBookCoverUrl.Should().BeNull();
    }
}