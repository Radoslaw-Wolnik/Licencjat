using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentAssertions;

namespace Tests.Domain.Entities;

public class UserBookTests
{
    private readonly Guid _id = Guid.NewGuid();
    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly Guid _generalBookId = Guid.NewGuid();
    private const BookStatus Status = BookStatus.Finished;
    private const BookState State = BookState.Available;
    private LanguageCode Language = LanguageCode.Create("en").ValueOrDefault;
    private const int PageCount = 300;
    private readonly Photo _coverPhoto = new("cover.jpg");

    [Fact]
    public void Create_WithValidParameters_CreatesUserBook()
    {
        // Act
        var result = UserBook.Create(_id, _ownerId, _generalBookId, Status, State, Language, PageCount, _coverPhoto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Id = _id,
            OwnerId = _ownerId,
            GeneralBookId = _generalBookId,
            Status,
            State,
            Language,
            PageCount,
            CoverPhoto = _coverPhoto
        });
    }

    [Fact]
    public void Create_WithInvalidOwnerId_ReturnsError()
    {
        // Act
        var result = UserBook.Create(_id, Guid.Empty, _generalBookId, Status, State, Language, PageCount, _coverPhoto);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("User"));
    }

    [Fact]
    public void Reconstitute_CreatesUserBookWithBookmarks()
    {
        // Arrange
        var bookmark = Bookmark.Create(
            Guid.NewGuid(), Guid.NewGuid(), BookmarkColours.red, 1, "Note").Value;
        
        // Act
        var userBook = UserBook.Reconstitute(
            _id, _ownerId, _generalBookId, Status, State, Language, PageCount, _coverPhoto, 
            new[] { bookmark });

        // Assert
        userBook.IsFailed.Should().BeFalse();
        userBook.Value.Bookmarks.Should().ContainSingle().Which.Should().BeEquivalentTo(bookmark);
    }

    [Fact]
    public void UpdateStatus_ChangesState()
    {
        // Arrange
        var userBook = UserBook.Create(_id, _ownerId, _generalBookId, Status, State, Language, PageCount, _coverPhoto).Value;
        var newStatus = BookState.Borrowed;
        
        // Act
        userBook.UpdateState(newStatus);
        
        // Assert
        userBook.State.Should().Be(newStatus);
    }

    [Fact]
    public void UpdateCover_ChangesCoverPhoto()
    {
        // Arrange
        var userBook = UserBook.Create(_id, _ownerId, _generalBookId, Status, State, Language, PageCount, _coverPhoto).Value;
        var newCover = new Photo("new-cover.jpg");
        
        // Act
        userBook.UpdateCover(newCover);
        
        // Assert
        userBook.CoverPhoto.Should().BeEquivalentTo(newCover);
    }
}