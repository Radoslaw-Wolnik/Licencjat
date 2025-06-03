using Backend.Domain.Collections;
using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentAssertions;

namespace Tests.Domain.Collections;

public class ReviewsCollectionTests
{
    private readonly Review _review1;
    private readonly Review _review2;

    public ReviewsCollectionTests()
    {
        _review1 = Review.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            DateTime.UtcNow,
            "Good book").Value;
        
        _review2 = Review.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            8,
            DateTime.UtcNow,
            "Great book").Value;
    }

    [Fact]
    public void Add_NewReview_AddsToCollection()
    {
        // Arrange
        var collection = new ReviewsCollection(new[] { _review1 });
        
        // Act
        var result = collection.Add(_review2);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Reviews.Should().BeEquivalentTo([_review1, _review2]);
    }

    [Fact]
    public void Add_DuplicateReview_ReturnsFailure()
    {
        // Arrange
        var collection = new ReviewsCollection(new[] { _review1 });
        
        // Act
        var result = collection.Add(_review1);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Already added this review.");
    }

    [Fact]
    public void Add_DuplicateUserReview_ReturnsFailure()
    {
        // Arrange
        var duplicateUserReview = Review.Create(
            Guid.NewGuid(),
            _review1.UserId, // Same user
            _review1.BookId,
            7,
            DateTime.UtcNow,
            "Another review").Value;
        
        var collection = new ReviewsCollection(new[] { _review1 });
        
        // Act
        var result = collection.Add(duplicateUserReview);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("One person can only add one review");
    }

    [Fact]
    public void Remove_ExistingReview_RemovesFromCollection()
    {
        // Arrange
        var collection = new ReviewsCollection(new[] { _review1, _review2 });
        
        // Act
        var result = collection.Remove(_review1.Id);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Reviews.Should().BeEquivalentTo([_review2]);
    }

    [Fact]
    public void Update_ValidReview_UpdatesCollection()
    {
        // Arrange
        var updated = Review.Create(
            _review1.Id,
            _review1.UserId,
            _review1.BookId,
            9, // Updated rating
            DateTime.UtcNow,
            "Updated comment").Value;
        
        var collection = new ReviewsCollection(new[] { _review1 });
        
        // Act
        var result = collection.Update(updated);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Reviews.Should().ContainSingle().Which.Should().BeEquivalentTo(updated);
    }

    [Fact]
    public void Update_ChangingUserId_ReturnsFailure()
    {
        // Arrange
        var updated = Review.Create(
            _review1.Id,
            Guid.NewGuid(), // Different user
            _review1.BookId,
            _review1.Rating,
            _review1.CreatedAt,
            _review1.Comment).Value;
        
        var collection = new ReviewsCollection(new[] { _review1 });
        
        // Act
        var result = collection.Update(updated);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("cannot update the user that made the review");
    }
}