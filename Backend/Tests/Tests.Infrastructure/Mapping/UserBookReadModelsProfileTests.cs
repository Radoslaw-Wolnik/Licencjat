using AutoMapper;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.UserBooks;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Mapping;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace Tests.Infrastructure.Mapping;
// fails 10 out of 11
public class UserBookReadModelsProfileTests
{
    private readonly IMapper _mapper;

    public UserBookReadModelsProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserBookReadModelProfile>();
            cfg.AddProfile<UserReadModelProfile>();
            cfg.AddProfile<GeneralBookReadModelProfile>();
        });

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void UserBookEntity_To_UserBookProjection_MapsCorrectly()
    {
        // Arrange
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "bookowner",
            ProfilePicture = "avatar.jpg",
            Reputation = 4.5f
        };

        var entity = new UserBookEntity
        {
            Id = Guid.NewGuid(),
            CoverPhoto = "cover.jpg",
            User = user,
            Book = new GeneralBookEntity
            {
                Title = "Clean Code",
                Author = "Robert C. Martin"
            }
        };

        // Act
        var projection = _mapper.Map<UserBookProjection>(entity);

        // Assert
        projection.Id.Should().Be(entity.Id);
        projection.Title.Should().Be("Clean Code");
        projection.Author.Should().Be("Robert C. Martin");
        projection.CoverPhoto.Should().Be("cover.jpg");
        projection.UserId.Should().Be(user.Id);
        projection.UserName.Should().Be("bookowner");
        projection.ProfilePictureUrl.Should().Be("avatar.jpg");
        projection.UserReputation.Should().Be(4.5f);
    }

    [Fact (Skip = "This is cured projection and should never have been made- we need to purge it")]
    public void UserBookProjection_To_UserBookListItem_MapsCorrectly()
    {
        // Arrange
        var projection = new UserBookProjection(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            Title: "Domain-Driven Design",
            Author: "Eric Evans",
            UserName: "dddfan",
            UserReputation: 4.8f,
            ProfilePictureUrl: "dddfan.jpg",
            CoverPhoto: "ddd-cover.jpg"
        );

        // Act
        var listItem = _mapper.Map<UserBookListItem>(projection);

        // Assert
        listItem.Id.Should().Be(projection.Id);
        listItem.Title.Should().Be(projection.Title);
        listItem.Author.Should().Be(projection.Author);
        listItem.CoverUrl.Should().Be(projection.CoverPhoto);
        listItem.State.Should().Be(BookState.Available); // Default value
        listItem.User.UserId.Should().Be(projection.UserId);
        listItem.User.Username.Should().Be("dddfan");
        listItem.User.ProfilePictureUrl.Should().Be("dddfan.jpg");
    }

    [Fact]
    public void UserBookEntity_To_UserLibraryListItem_MapsCorrectly()
    {
        // Arrange
        var entity = new UserBookEntity
        {
            Id = Guid.NewGuid(),
            CoverPhoto = "library-cover.jpg",
            State = BookState.Borrowed,
            Status = BookStatus.Finished,
            Book = new GeneralBookEntity
            {
                Title = "Refactoring",
                Author = "Martin Fowler",
                Reviews = new List<ReviewEntity>
                {
                    new() { Rating = 9 },
                    new() { Rating = 8 }
                }
            }
        };

        // Act
        var listItem = _mapper.Map<UserLibraryListItem>(entity);

        // Assert
        listItem.Id.Should().Be(entity.Id);
        listItem.Title.Should().Be("Refactoring");
        listItem.Author.Should().Be("Martin Fowler");
        listItem.CoverUrl.Should().Be("library-cover.jpg");
        listItem.State.Should().Be(BookState.Borrowed);
        listItem.Status.Should().Be(BookStatus.Finished);
        listItem.RatingAvg.Should().Be(8.5f);
    }

    [Fact]
    public void UserBookEntity_To_UserBookDetailsReadModel_MapsCorrectly()
    {
        // Arrange
        var entity = new UserBookEntity
        {
            Id = Guid.NewGuid(),
            Language = "en",
            CoverPhoto = "details-cover.jpg",
            PageCount = 400,
            State = BookState.Available,
            Status = BookStatus.Reading,
            Book = new GeneralBookEntity
            {
                Title = "Design Patterns",
                Author = "Gamma, Helm, Johnson, Vlissides"
            }
        };

        // Act
        var details = _mapper.Map<UserBookDetailsReadModel>(entity);

        // Assert
        details.Id.Should().Be(entity.Id);
        details.Title.Should().Be("Design Patterns");
        details.Author.Should().Be("Gamma, Helm, Johnson, Vlissides");
        details.LanguageCode.Should().Be("en");
        details.CoverPhotoUrl.Should().Be("details-cover.jpg");
        details.PageCount.Should().Be(400);
        details.State.Should().Be(BookState.Available);
        details.Status.Should().Be(BookStatus.Reading);
    }

    [Fact]
    public void BookmarkEntity_To_BookmarkReadModel_MapsCorrectly()
    {
        // Arrange
        var entity = new BookmarkEntity
        {
            Id = Guid.NewGuid(),
            Page = 123,
            Colour = BookmarkColours.blue,
            Description = "Important concept"
        };

        // Act
        var bookmark = _mapper.Map<BookmarkReadModel>(entity);

        // Assert
        bookmark.Id.Should().Be(entity.Id);
        bookmark.Page.Should().Be(123);
        bookmark.Colour.Should().Be(BookmarkColours.blue);
        bookmark.Description.Should().Be("Important concept");
    }

    [Fact]
    public void UserBookEntity_To_UserOwnBookProfileReadModel_MapsCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var swapId = Guid.NewGuid();

        var entity = new UserBookEntity
        {
            Id = Guid.NewGuid(),
            Language = "pl",
            CoverPhoto = "own-book-cover.jpg",
            PageCount = 350,
            State = BookState.Available,
            Status = BookStatus.Finished,
            UserId = userId,
            Book = new GeneralBookEntity
            {
                Title = "The Pragmatic Programmer",
                Author = "Hunt and Thomas",
                Reviews = new List<ReviewEntity>
                {
                    new()
                    {
                        UserId = userId,
                        Rating = 10,
                        Comment = "Changed my career",
                        CreatedAt = DateTime.UtcNow
                    }
                }
            },
            Bookmarks = new List<BookmarkEntity>
            {
                new() { Page = 50, Colour = BookmarkColours.red },
                new() { Page = 200, Description = "Key insight" }
            },
            SubSwaps = new List<SubSwapEntity>
            {
                new()
                {
                    Swap = new SwapEntity
                    {
            Id = swapId,
            Status = SwapStatus.Ongoing,
            CreatedAt = new DateOnly(2023, 6, 1),
            SubSwapRequesting = new SubSwapEntity
            {
                UserId = userId,
                UserBookReading = new UserBookEntity { CoverPhoto = "requesting-cover.jpg" }
            },
            SubSwapAccepting = new SubSwapEntity
            {
                UserId = Guid.NewGuid(),
                User = new UserEntity { UserName = "otheruser" }
            },
            TimelineUpdates = new List<TimelineEntity>
            {
                new() { Status = TimelineStatus.ReadingBooks }
            }
        }
                }
            }
        };

        // Act
        var profileModel = _mapper.Map<UserOwnBookProfileReadModel>(entity,
        opts => opts.Items["CurrentUserId"] = userId);

        // Assert
        profileModel.Id.Should().Be(entity.Id);
        profileModel.Title.Should().Be("The Pragmatic Programmer");
        profileModel.Author.Should().Be("Hunt and Thomas");
        profileModel.LanguageCode.Should().Be("pl");
        profileModel.CoverPhotoUrl.Should().Be("own-book-cover.jpg");
        profileModel.PageCount.Should().Be(350);
        profileModel.State.Should().Be(BookState.Available);
        profileModel.Status.Should().Be(BookStatus.Finished);

        profileModel.UserReview.Should().NotBeNull();
        profileModel.UserReview.Rating.Should().Be(10);

        profileModel.Swaps.Should().HaveCount(1);
        profileModel.Swaps.First().Id.Should().Be(swapId);
        profileModel.Swaps.First().Status.Should().Be(SwapStatus.Ongoing);

        profileModel.Bookmarks.Should().HaveCount(2);
        profileModel.Bookmarks.First().Page.Should().Be(200); // Ordered descending
    }

    [Fact]
    public void SwapEntity_To_SwapUserBookListItem_MapsCorrectly_ForRequestingUser()
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
                UserBookReading = new UserBookEntity { CoverPhoto = "requesting-cover.jpg" }
            },
            SubSwapAccepting = new SubSwapEntity
            {
                UserId = otherUserId,
                User = new UserEntity { UserName = "otheruser" }
            },
            TimelineUpdates = new List<TimelineEntity>
            {
                new() { Status = TimelineStatus.ReadingBooks }
            }
        };

        // Act
        var listItem = _mapper.Map<SwapUserBookListItem>(swap, opts =>
            opts.Items["CurrentUserId"] = currentUserId);

        // Assert
        listItem.Id.Should().Be(swap.Id);
        listItem.Username.Should().Be("otheruser");
        listItem.CoverPhotoUrl.Should().Be("requesting-cover.jpg");
        listItem.CreatedAt.Should().Be(swap.CreatedAt);
        listItem.Status.Should().Be(SwapStatus.Ongoing);
    }

    [Fact]
    public void SwapEntity_To_SwapUserBookListItem_MapsCorrectly_ForAcceptingUser()
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
                User = new UserEntity { UserName = "requester" },
                UserBookReading = new UserBookEntity { CoverPhoto = "requesting-cover.jpg" }
            },
            SubSwapAccepting = new SubSwapEntity
            {
                UserId = currentUserId,
                UserBookReading = new UserBookEntity { CoverPhoto = "accepting-cover.jpg" }
            },
            TimelineUpdates = new List<TimelineEntity>
            {
                new() { Status = TimelineStatus.Requested }
            }
        };

        // Act
        var listItem = _mapper.Map<SwapUserBookListItem>(swap, opts =>
            opts.Items["CurrentUserId"] = currentUserId);

        // Assert
        listItem.Username.Should().Be("requester");
        listItem.CoverPhotoUrl.Should().Be("accepting-cover.jpg");
        listItem.Status.Should().Be(SwapStatus.Requested);
    }

    [Fact]
    public void UserOwnBookProfileReadModel_LimitsBookmarksTo10()
    {
        // Arrange
        var entity = new UserBookEntity
        {
            Bookmarks = Enumerable.Range(1, 15)
                .Select(i => new BookmarkEntity { Page = i })
                .ToList(),
            Book = new GeneralBookEntity(),
            SubSwaps = new List<SubSwapEntity>()
        };

        // Act
        var profileModel = _mapper.Map<UserOwnBookProfileReadModel>(entity);

        // Assert
        profileModel.Bookmarks.Should().HaveCount(10);
        profileModel.Bookmarks.Select(b => b.Page)
            .Should().ContainInOrder(15, 14, 13, 12, 11, 10, 9, 8, 7, 6);
    }

    [Fact]
    public void UserOwnBookProfileReadModel_HandlesMissingReview()
    {
        // Arrange
        var entity = new UserBookEntity
        {
            UserId = Guid.NewGuid(),
            Book = new GeneralBookEntity
            {
                Reviews = new List<ReviewEntity>()
            },
            Bookmarks = new List<BookmarkEntity>(),
            SubSwaps = new List<SubSwapEntity>()
        };

        // Act
        var profileModel = _mapper.Map<UserOwnBookProfileReadModel>(entity);

        // Assert
        profileModel.UserReview.Should().BeNull();
    }

    
    [Fact]
    public void UserBookEntity_To_BookCoverItemReadModel_MapsCorrectly()
    {
        // Arrange
        var entity = new UserBookEntity
        {
            Id = Guid.NewGuid(),
            CoverPhoto = "userbook-cover.jpg",
            Book = new GeneralBookEntity { Title = "User Book" }
        };

        // Act
        var coverModel = _mapper.Map<BookCoverItemReadModel>(entity);

        // Assert
        coverModel.Id.Should().Be(entity.Id);
        coverModel.Title.Should().Be("User Book");
        coverModel.CoverUrl.Should().Be("userbook-cover.jpg");
    }
}