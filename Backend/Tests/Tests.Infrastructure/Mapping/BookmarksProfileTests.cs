using AutoMapper;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Mapping;
using FluentAssertions;
using FluentResults;

namespace Tests.Infrastructure.Mapping;

public class BookmarkProfileTests
{
    private readonly IMapper _mapper;

    public BookmarkProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<BookmarkProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void BookmarkEntity_To_Bookmark_MapsCorrectly()
    {
        // Arrange
        var entity = new BookmarkEntity
        {
            Id = Guid.NewGuid(),
            UserBookId = Guid.NewGuid(),
            Colour = BookmarkColours.green,
            Page = 42,
            Description = "Important discovery"
        };

        // Act
        var bookmark = _mapper.Map<Bookmark>(entity);

        // Assert
        bookmark.Id.Should().Be(entity.Id);
        bookmark.UserBookId.Should().Be(entity.UserBookId);
        bookmark.Colour.Should().Be(entity.Colour);
        bookmark.Page.Should().Be(entity.Page);
        bookmark.Description.Should().Be(entity.Description);
    }

    [Fact]
    public void Bookmark_To_BookmarkEntity_MapsCorrectly()
    {
        // Arrange
        var bookmark = Bookmark.Create(
            id: Guid.NewGuid(),
            userBookId: Guid.NewGuid(),
            colour: BookmarkColours.blue,
            page: 150,
            description: "Character development"
        ).Value;

        // Act
        var entity = _mapper.Map<BookmarkEntity>(bookmark);

        // Assert
        entity.Id.Should().Be(bookmark.Id);
        entity.UserBookId.Should().Be(bookmark.UserBookId);
        entity.Colour.Should().Be(bookmark.Colour);
        entity.Page.Should().Be(bookmark.Page);
        entity.Description.Should().Be(bookmark.Description);
    }

    [Fact]
    public void BookmarkEntity_To_Bookmark_HandlesNullDescription()
    {
        // Arrange
        var entity = new BookmarkEntity
        {
            Id = Guid.NewGuid(),
            UserBookId = Guid.NewGuid(),
            Colour = BookmarkColours.red,
            Page = 99,
            Description = null
        };

        // Act
        var bookmark = _mapper.Map<Bookmark>(entity);

        // Assert
        bookmark.Description.Should().BeNull();
    }

    [Fact]
    public void Bookmark_To_BookmarkEntity_HandlesNullDescription()
    {
        // Arrange
        var bookmark = Bookmark.Create(
            id: Guid.NewGuid(),
            userBookId: Guid.NewGuid(),
            colour: BookmarkColours.yesllow,
            page: 200,
            description: null
        ).Value;

        // Act
        var entity = _mapper.Map<BookmarkEntity>(bookmark);

        // Assert
        entity.Description.Should().BeNull();
    }

    [Fact]
    public void BookmarkEntity_To_Bookmark_ThrowsForInvalidUserBookId()
    {
        // Arrange
        var entity = new BookmarkEntity
        {
            Id = Guid.NewGuid(),
            UserBookId = Guid.Empty,  // Invalid
            Colour = BookmarkColours.pinky,
            Page = 10,
            Description = "Invalid"
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<Bookmark>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*NotFound*UserBook*");
    }

    [Fact]
    public void BookmarkEntity_To_Bookmark_ThrowsForInvalidColour()
    {
        // Arrange
        var entity = new BookmarkEntity
        {
            Id = Guid.NewGuid(),
            UserBookId = Guid.NewGuid(),
            Colour = (BookmarkColours)999,  // Invalid enum value
            Page = 10,
            Description = "Invalid colour"
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<Bookmark>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*Wrong colour*");
    }

    [Fact]
    public void BookmarkEntity_To_Bookmark_ThrowsForInvalidPage()
    {
        // Arrange
        var entity = new BookmarkEntity
        {
            Id = Guid.NewGuid(),
            UserBookId = Guid.NewGuid(),
            Colour = BookmarkColours.brown,
            Page = -5,  // Invalid
            Description = "Negative page"
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<Bookmark>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*Page must be above 0*");
    }

    [Fact]
    public void Bookmark_To_BookmarkEntity_DoesNotMapNavigationProperties()
    {
        // Arrange
        var bookmark = Bookmark.Create(
            id: Guid.NewGuid(),
            userBookId: Guid.NewGuid(),
            colour: BookmarkColours.pinky,
            page: 75,
            description: "Should ignore navigation"
        ).Value;

        // Act
        var entity = _mapper.Map<BookmarkEntity>(bookmark);

        // Assert
        entity.UserBook.Should().BeNull();
    }
}