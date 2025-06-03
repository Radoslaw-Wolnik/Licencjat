using Backend.Domain.Collections;
using FluentAssertions;
using FluentResults;

namespace Tests.Domain.Collections;

public class WishlistCollectionTests
{
    private readonly Guid _bookId1 = Guid.NewGuid();
    private readonly Guid _bookId2 = Guid.NewGuid();

    [Fact]
    public void Add_NewBook_AddsToWishlist()
    {
        // Arrange
        var collection = new WishlistCollection(new[] { _bookId1 });
        
        // Act
        var result = collection.Add(_bookId2);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Items.Should().BeEquivalentTo([_bookId1, _bookId2]);
    }

    [Fact]
    public void Add_ExistingBook_ReturnsFailure()
    {
        // Arrange
        var collection = new WishlistCollection(new[] { _bookId1 });
        
        // Act
        var result = collection.Add(_bookId1);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Already in wishlist.");
    }

    [Fact]
    public void Remove_ExistingBook_RemovesFromWishlist()
    {
        // Arrange
        var collection = new WishlistCollection(new[] { _bookId1, _bookId2 });
        
        // Act
        var result = collection.Remove(_bookId1);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Items.Should().BeEquivalentTo([_bookId2]);
    }
}