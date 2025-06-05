using AutoMapper;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Mapping;
using FluentAssertions;
using FluentResults;

namespace Tests.Infrastructure.Mapping;

public class SubSwapProfileTests
{
    private readonly IMapper _mapper;

    public SubSwapProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SubSwapProfile>();
            cfg.AddProfile<UserBookProfile>();
            cfg.AddProfile<FeedbackProfile>();
            cfg.AddProfile<IssueProfile>();
        });
        
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void SubSwapEntity_To_SubSwap_MapsCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var subSwapId = Guid.NewGuid();
        var entity = new SubSwapEntity
        {
            Id = subSwapId,
            UserId = userId,
            PageAt = 100,
            UserBookReading = new UserBookEntity
            {
                Id = Guid.NewGuid(),
                Language = "en",
                PageCount = 300,
                CoverPhoto = "cover.jpg",
                Status = BookStatus.Reading,
                State = BookState.Available,
                UserId = userId,
                BookId = bookId
            },
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
                Id = Guid.NewGuid(),
                UserId = userId,
                SubSwapId = subSwapId,
                Description = "Minor damage to cover"
            }
        };

        // Act
        var subSwap = _mapper.Map<SubSwap>(entity);

        // Assert
        subSwap.Id.Should().Be(entity.Id);
        subSwap.UserId.Should().Be(entity.UserId);
        subSwap.PageAt.Should().Be(entity.PageAt);
        
        subSwap.UserBookReading.Should().NotBeNull();
        subSwap.UserBookReading.OwnerId.Should().Be(userId);
        subSwap.UserBookReading.PageCount.Should().Be(300);
        
        subSwap.Feedback.Should().NotBeNull();
        subSwap.Feedback.Stars.Should().Be(5);
        subSwap.Feedback.Recommend.Should().BeTrue();
        
        subSwap.Issue.Should().NotBeNull();
        subSwap.Issue.Description.Should().Be("Minor damage to cover");
    }

    [Fact]
    public void SubSwap_To_SubSwapEntity_MapsCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var userBookId = Guid.NewGuid();
        var subSwapId = Guid.NewGuid();
        var subSwap = SubSwap.Create(
            id: subSwapId,
            userId: userId,
            pageAt: 150,
            userBookReading: UserBook.Reconstitute(
                id: userBookId,
                ownerId: userId,
                generalBookId: bookId,
                status: BookStatus.Finished,
                state: BookState.Borrowed,
                language: LanguageCode.Create("pl").Value,
                pageCount: 400,
                coverPhoto: new Photo("cover.jpg"),
                bookmarks: Enumerable.Empty<Bookmark>()
            ).Value,
            feedback: new Feedback(
                Guid.NewGuid(),
                subSwapId,
                userId,
                4,
                true,
                SwapLength.TooShort,
                SwapConditionBook.Better,
                SwapCommunication.Okay
            ),
            issue: null
        ).Value;

        // Act
        var entity = _mapper.Map<SubSwapEntity>(subSwap);

        // Assert
        entity.Id.Should().Be(subSwap.Id);
        entity.UserId.Should().Be(subSwap.UserId);
        entity.PageAt.Should().Be(subSwap.PageAt);
        entity.UserBookReadingId.Should().Be(userBookId);
        entity.FeedbackId.Should().Be(subSwap.Feedback?.Id);
        entity.IssueId.Should().BeNull();
    }

    [Fact]
    public void SubSwapEntity_To_SubSwap_HandlesNullValues()
    {
        // Arrange
        var entity = new SubSwapEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            PageAt = 0,
            UserBookReading = null,
            Feedback = null,
            Issue = null
        };

        // Act
        var subSwap = _mapper.Map<SubSwap>(entity);

        // Assert
        subSwap.UserBookReading.Should().BeNull();
        subSwap.Feedback.Should().BeNull();
        subSwap.Issue.Should().BeNull();
    }

    [Fact]
    public void SubSwap_To_SubSwapEntity_HandlesNullValues()
    {
        // Arrange
        var subSwap = SubSwap.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            pageAt: 0,
            userBookReading: null,
            feedback: null,
            issue: null
        ).Value;

        // Act
        var entity = _mapper.Map<SubSwapEntity>(subSwap);

        // Assert
        entity.UserBookReadingId.Should().BeNull();
        entity.FeedbackId.Should().BeNull();
        entity.IssueId.Should().BeNull();
    }

    [Fact]
    public void SubSwapEntity_To_SubSwap_ThrowsForInvalidPageAt()
    {
        // Arrange
        var entity = new SubSwapEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            PageAt = -10 // Invalid
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<SubSwap>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*Page must be above 0 or equal 0*");
    }
}