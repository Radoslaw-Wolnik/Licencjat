using Backend.Domain.Collections;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using FluentAssertions;
using FluentResults;
using Xunit;

namespace Tests.Domain.Collections;

public class BookmarksCollectionTests
{
    private readonly Bookmark _bookmark1 = Bookmark.Create(
        Guid.NewGuid(), Guid.NewGuid(), BookmarkColours.red, 1, "Note1").Value;
    
    private readonly Bookmark _bookmark2 = Bookmark.Create(
        Guid.NewGuid(), Guid.NewGuid(), BookmarkColours.blue, 2, "Note2").Value;

    [Fact]
    public void Add_NewBookmark_AddsToCollection()
    {
        // Arrange
        var collection = new BookmarksCollection(new[] { _bookmark1 });
        
        // Act
        var result = collection.Add(_bookmark2);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Bookmarks.Should().BeEquivalentTo([_bookmark1, _bookmark2]);
    }

    [Fact]
    public void Add_DuplicateBookmark_ReturnsFailure()
    {
        // Arrange
        var collection = new BookmarksCollection(new[] { _bookmark1 });
        
        // Act
        var result = collection.Add(_bookmark1);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Already added this bookmark.");
    }

    [Fact]
    public void Add_DuplicatePageAndDescription_ReturnsFailure()
    {
        // Arrange
        var duplicate = Bookmark.Create(
            Guid.NewGuid(), 
            _bookmark1.UserBookId, 
            BookmarkColours.green, 
            _bookmark1.Page, 
            _bookmark1.Description).Value;
        
        var collection = new BookmarksCollection(new[] { _bookmark1 });
        
        // Act
        var result = collection.Add(duplicate);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Duplicate bookmark");
    }

    [Fact]
    public void Remove_ExistingBookmark_RemovesFromCollection()
    {
        // Arrange
        var collection = new BookmarksCollection(new[] { _bookmark1, _bookmark2 });
        
        // Act
        var result = collection.Remove(_bookmark1.Id);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Bookmarks.Should().BeEquivalentTo([_bookmark2]);
    }

    [Fact]
    public void Remove_NonExistentBookmark_ReturnsFailure()
    {
        // Arrange
        var collection = new BookmarksCollection(new[] { _bookmark1 });
        
        // Act
        var result = collection.Remove(_bookmark2.Id);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Bookmark not found in the bookmarks of this book");
    }

    [Fact]
    public void Update_ValidBookmark_UpdatesCollection()
    {
        // Arrange
        var updated = Bookmark.Create(
            _bookmark1.Id,
            _bookmark1.UserBookId,
            BookmarkColours.green,
            10,
            "Updated").Value;
        
        var collection = new BookmarksCollection(new[] { _bookmark1 });
        
        // Act
        var result = collection.Update(updated);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Bookmarks.Should().ContainSingle().Which.Should().BeEquivalentTo(updated);
    }

    [Fact]
    public void Update_ChangingUserBookId_ReturnsFailure()
    {
        // Arrange
        var updated = Bookmark.Create(
            _bookmark1.Id,
            Guid.NewGuid(), // Different UserBookId
            _bookmark1.Colour,
            _bookmark1.Page,
            _bookmark1.Description).Value;
        
        var collection = new BookmarksCollection(new[] { _bookmark1 });
        
        // Act
        var result = collection.Update(updated);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("cannot update the userBook of the bookmark");
    }
}