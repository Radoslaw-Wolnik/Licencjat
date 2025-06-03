using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Tests.Domain.Helpers;

namespace Tests.Domain.Common;

public class BookmarkTests
{
    private readonly Guid _validId = Guid.NewGuid();
    private readonly Guid _validUserBookId = Guid.NewGuid();
    private const BookmarkColours _validColour = BookmarkColours.red;
    private const int _validPage = 1;

    [Fact]
    public void Create_WithValidParameters_ReturnsBookmark()
    {
        // Act
        var result = Bookmark.Create(
            _validId, _validUserBookId, _validColour, _validPage, "Note");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Id = _validId,
            UserBookId = _validUserBookId,
            Colour = _validColour,
            Page = _validPage,
            Description = "Note"
        });
    }

    [Theory]
    [InlineData(BookmarkColours.red)]
    [InlineData(BookmarkColours.blue)]
    [InlineData(BookmarkColours.green)]
    public void Create_WithValidColour_ReturnsBookmark(BookmarkColours colour)
    {
        // Act
        var result = Bookmark.Create(
            _validId, _validUserBookId, colour, _validPage, null);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidUserBookId_ReturnsError()
    {
        // Act
        var result = Bookmark.Create(
            _validId, Guid.Empty, _validColour, _validPage, null);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeNotFoundError("UserBook", Guid.Empty);
    }

    [Fact]
    public void Create_WithInvalidColour_ReturnsError()
    {
        // Act
        var result = Bookmark.Create(
            _validId, _validUserBookId, (BookmarkColours)100, _validPage, null);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("Bookmark", "Wrong colour");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidPage_ReturnsError(int page)
    {
        // Act
        var result = Bookmark.Create(
            _validId, _validUserBookId, _validColour, page, null);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("Bookmark", "Page must be above 0");
    }
}