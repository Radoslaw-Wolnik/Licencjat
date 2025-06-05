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

public class SwapProfileTests
{
    private readonly IMapper _mapper;

    public SwapProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SwapProfile>();
            cfg.AddProfile<SubSwapProfile>();
            cfg.AddProfile<MeetupProfile>();
            cfg.AddProfile<TimelineProfile>();
            cfg.AddProfile<UserBookProfile>();
            cfg.AddProfile<FeedbackProfile>();
            cfg.AddProfile<IssueProfile>();
        });
        
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void SwapEntity_To_Swap_MapsCorrectly()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var bookId1 = Guid.NewGuid();
        var bookId2 = Guid.NewGuid();
        
        var entity = new SwapEntity
        {
            Id = Guid.NewGuid(),
            Status = SwapStatus.Requested,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            SubSwapRequesting = new SubSwapEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId1,
                PageAt = 50,
                UserBookReading = new UserBookEntity
                {
                    Id = Guid.NewGuid(),
                    Language = "en",
                    PageCount = 300,
                    CoverPhoto = "cover1.jpg",
                    Status = BookStatus.Reading,
                    State = BookState.Available,
                    UserId = userId1,
                    BookId = bookId1
                }
            },
            SubSwapAccepting = new SubSwapEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId2,
                PageAt = 100,
                UserBookReading = new UserBookEntity
                {
                    Id = Guid.NewGuid(),
                    Language = "pl",
                    PageCount = 400,
                    CoverPhoto = "cover2.jpg",
                    Status = BookStatus.Finished,
                    State = BookState.Borrowed,
                    UserId = userId2,
                    BookId = bookId2
                }
            },
            Meetups = new List<MeetupEntity>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Location_X = 52.2297,
                    Location_Y = (float)21.0122, // 21.01219940185547
                    Status = MeetupStatus.Proposed,
                    SuggestedUserId = userId1
                }
            },
            TimelineUpdates = new List<TimelineEntity>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Status = TimelineStatus.Requested,
                    Description = "Swap requested",
                    UserId = userId1,
                    CreatedAt = DateTime.UtcNow
                }
            }
        };

        // Act
        var swap = _mapper.Map<Swap>(entity);

        // Assert
        swap.Id.Should().Be(entity.Id);
        swap.Status.Should().Be(entity.Status);
        swap.CreatedAt.Should().Be(entity.CreatedAt);
        
        swap.SubSwapRequesting.Should().NotBeNull();
        swap.SubSwapRequesting.UserId.Should().Be(userId1);
        swap.SubSwapRequesting.PageAt.Should().Be(50);
        swap.SubSwapRequesting.UserBookReading.Should().NotBeNull();
        swap.SubSwapRequesting.UserBookReading.OwnerId.Should().Be(userId1);
        
        swap.SubSwapAccepting.Should().NotBeNull();
        swap.SubSwapAccepting.UserId.Should().Be(userId2);
        swap.SubSwapAccepting.PageAt.Should().Be(100);
        swap.SubSwapAccepting.UserBookReading.Should().NotBeNull();
        swap.SubSwapAccepting.UserBookReading.OwnerId.Should().Be(userId2);
        
        swap.Meetups.Should().HaveCount(1);
        swap.Meetups.First().Location.Latitude.Should().Be(52.2297);
        swap.Meetups.First().Location.Longitude.Should().Be(21.0122f);
        
        swap.TimelineUpdates.Should().HaveCount(1);
        swap.TimelineUpdates.First().Description.Should().Be("Swap requested");
    }

    [Fact]
    public void Swap_To_SwapEntity_MapsCorrectly()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var bookId1 = Guid.NewGuid();
        var bookId2 = Guid.NewGuid();
        
        var swap = Swap.Reconstitute(
            id: Guid.NewGuid(),
            status: SwapStatus.Ongoing,
            subSwapRequesitng: SubSwap.Create(
                id: Guid.NewGuid(),
                userId: userId1,
                pageAt: 75,
                userBookReading: UserBook.Reconstitute(
                    id: Guid.NewGuid(),
                    ownerId: userId1,
                    generalBookId: bookId1,
                    status: BookStatus.Reading,
                    state: BookState.Available,
                    language: LanguageCode.Create("en").Value,
                    pageCount: 300,
                    coverPhoto: Photo.Create("cover1.jpg").Value,
                    bookmarks: Enumerable.Empty<Bookmark>()
                ).Value,
                feedback: null,
                issue: null
            ).Value,
            subSwapAccepting: SubSwap.Create(
                id: Guid.NewGuid(),
                userId: userId2,
                pageAt: 150,
                userBookReading: UserBook.Reconstitute(
                    id: Guid.NewGuid(),
                    ownerId: userId2,
                    generalBookId: bookId2,
                    status: BookStatus.Finished,
                    state: BookState.Borrowed,
                    language: LanguageCode.Create("pl").Value,
                    pageCount: 400,
                    coverPhoto: Photo.Create("cover2.jpg").Value,
                    bookmarks: Enumerable.Empty<Bookmark>()
                ).Value,
                feedback: null,
                issue: null
            ).Value,
            meetups: new List<Meetup>
            {
                new(
                    Guid.NewGuid(),
                    Guid.NewGuid(), // swapId
                    userId1,
                    MeetupStatus.Confirmed,
                    LocationCoordinates.Create(51.5074, -0.1278).Value
                )
            },
            timelineUpdates: new List<TimelineUpdate>
            {
                new(
                    Guid.NewGuid(),
                    userId1,
                    Guid.NewGuid(), // swapId
                    TimelineStatus.Accepted,
                    "Swap accepted",
                    DateTime.UtcNow
                )
            },
            createdAt: DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        var entity = _mapper.Map<SwapEntity>(swap);

        // Assert
        entity.Id.Should().Be(swap.Id);
        entity.Status.Should().Be(swap.Status);
        entity.CreatedAt.Should().Be(swap.CreatedAt);
        
        entity.SubSwapRequestingId.Should().Be(swap.SubSwapRequesting.Id);
        entity.SubSwapAcceptingId.Should().Be(swap.SubSwapAccepting.Id);
        
        // Collections should be ignored in the mapping
        entity.Meetups.Should().BeEmpty();
        entity.TimelineUpdates.Should().BeEmpty();
    }

    [Fact]
    public void SwapEntity_To_Swap_HandlesNullSubSwapAccepting()
    {
        // Arrange
        var entity = new SwapEntity
        {
            Id = Guid.NewGuid(),
            Status = SwapStatus.Requested,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            SubSwapRequesting = new SubSwapEntity
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                PageAt = 0,
                UserBookReading = null
            },
            SubSwapAccepting = new SubSwapEntity {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                PageAt = 0,
                UserBookReading = null
            }
        };

        // Act
        var swap = _mapper.Map<Swap>(entity);

        // Assert
        swap.Id.Should().Be(entity.Id);
        swap.SubSwapRequesting.Should().NotBeNull();
        swap.SubSwapAccepting.Should().NotBeNull();
        swap.SubSwapAccepting.UserId.Should().NotBeEmpty();
        swap.SubSwapAccepting.UserBookReading.Should().BeNull();
    }

    [Fact]
    public void SwapEntity_To_Swap_HandlesEmptyCollections()
    {
        // Arrange
        var entity = new SwapEntity
        {
            Id = Guid.NewGuid(),
            Status = SwapStatus.Requested,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            SubSwapRequesting = new SubSwapEntity
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            },
            SubSwapAccepting = new SubSwapEntity
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            },
            Meetups = new List<MeetupEntity>(),
            TimelineUpdates = new List<TimelineEntity>()
        };

        // Act
        var swap = _mapper.Map<Swap>(entity);

        // Assert
        swap.Meetups.Should().BeEmpty();
        swap.TimelineUpdates.Should().BeEmpty();
    }

    [Fact]
    public void Swap_To_SwapEntity_MapsIdsCorrectly()
    {
        // Arrange
        var swapId = Guid.NewGuid();
        var requestingSubSwapId = Guid.NewGuid();
        var acceptingSubSwapId = Guid.NewGuid();
        
        var swap = Swap.Reconstitute(
            id: swapId,
            status: SwapStatus.Completed,
            subSwapRequesitng: SubSwap.Create(
                id: requestingSubSwapId,
                userId: Guid.NewGuid(),
                pageAt: 0,
                userBookReading: null,
                feedback: null,
                issue: null
            ).Value,
            subSwapAccepting: SubSwap.Create(
                id: acceptingSubSwapId,
                userId: Guid.NewGuid(),
                pageAt: 0,
                userBookReading: null,
                feedback: null,
                issue: null
            ).Value,
            meetups: Enumerable.Empty<Meetup>(),
            timelineUpdates: Enumerable.Empty<TimelineUpdate>(),
            createdAt: DateOnly.MinValue
        );

        // Act
        var entity = _mapper.Map<SwapEntity>(swap);

        // Assert
        entity.Id.Should().Be(swapId);
        entity.SubSwapRequestingId.Should().Be(requestingSubSwapId);
        entity.SubSwapAcceptingId.Should().Be(acceptingSubSwapId);
    }

    [Fact]
    public void SwapEntity_To_Swap_HandlesNullFeedbackAndIssue()
    {
        // Arrange
        var entity = new SwapEntity
        {
            Id = Guid.NewGuid(),
            SubSwapRequesting = new SubSwapEntity
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Feedback = null,
                Issue = null
            },
            SubSwapAccepting = new SubSwapEntity
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Feedback = null,
                Issue = null
            }
        };

        // Act
        var swap = _mapper.Map<Swap>(entity);

        // Assert
        swap.SubSwapRequesting.Feedback.Should().BeNull();
        swap.SubSwapRequesting.Issue.Should().BeNull();
        swap.SubSwapAccepting.Feedback.Should().BeNull();
        swap.SubSwapAccepting.Issue.Should().BeNull();
    }

    [Fact]
    public void SwapEntity_To_Swap_HandlesFeedbackAndIssue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user2Id = Guid.NewGuid();
        var subSwapId = Guid.NewGuid();
        var subSwap2Id = Guid.NewGuid();
        var entity = new SwapEntity
        {
            Id = Guid.NewGuid(),
            SubSwapRequesting = new SubSwapEntity
            {
                Id = subSwapId,
                UserId = userId,
                Feedback = new FeedbackEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    SubSwapId = subSwapId,
                    Stars = 5,
                    Recommend = true,
                    Lenght = SwapLength.JustRight,
                    ConditionBook = SwapConditionBook.Same,
                    Communication = SwapCommunication.Perfect
                },
                Issue = new IssueEntity
                {
                    UserId = userId,
                    SubSwapId = subSwapId,
                    Id = Guid.NewGuid(),
                    Description = "Book damaged"
                }
            },
            SubSwapAccepting = new SubSwapEntity
            {
                Id = subSwap2Id,
                UserId = user2Id,
                Feedback = new FeedbackEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = user2Id,
                    SubSwapId = subSwap2Id,
                    Stars = 4,
                    Recommend = true,
                    Lenght = SwapLength.TooShort,
                    ConditionBook = SwapConditionBook.Better,
                    Communication = SwapCommunication.Okay
                }
            }
        };

        // Act
        var swap = _mapper.Map<Swap>(entity);

        // Assert
        swap.SubSwapRequesting.Feedback.Should().NotBeNull();
        swap.SubSwapRequesting.Feedback.Stars.Should().Be(5);
        swap.SubSwapRequesting.Issue.Should().NotBeNull();
        swap.SubSwapRequesting.Issue.Description.Should().Be("Book damaged");
        
        swap.SubSwapAccepting.Feedback.Should().NotBeNull();
        swap.SubSwapAccepting.Feedback.Stars.Should().Be(4);
        swap.SubSwapAccepting.Issue.Should().BeNull();
    }
}
