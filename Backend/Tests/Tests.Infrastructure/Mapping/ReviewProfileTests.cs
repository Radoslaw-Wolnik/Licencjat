using AutoMapper;
using Backend.Domain.Common;
using Backend.Domain.Errors;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Mapping;
using FluentAssertions;
using FluentResults;

namespace Tests.Infrastructure.Mapping;

public class ReviewProfileTests
{
    private readonly IMapper _mapper;

    public ReviewProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GeneralBookProfile>();
            cfg.AddProfile<ReviewProfile>();
        });
        
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void ReviewEntity_To_Review_MapsCorrectly()
    {
        // Arrange
        var reviewEntity = new ReviewEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BookId = Guid.NewGuid(),
            Rating = 8,
            Comment = "Great book!",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var review = _mapper.Map<Review>(reviewEntity);

        // Assert
        review.Id.Should().Be(reviewEntity.Id);
        review.UserId.Should().Be(reviewEntity.UserId);
        review.BookId.Should().Be(reviewEntity.BookId);
        review.Rating.Should().Be(reviewEntity.Rating);
        review.Comment.Should().Be(reviewEntity.Comment);
        review.CreatedAt.Should().Be(reviewEntity.CreatedAt);
    }

    [Fact]
    public void Review_To_ReviewEntity_MapsCorrectly()
    {
        // Arrange
        var review = Review.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            9,
            DateTime.UtcNow,
            "Excellent read"
        ).Value;

        // Act
        var reviewEntity = _mapper.Map<ReviewEntity>(review);

        // Assert
        reviewEntity.Id.Should().Be(review.Id);
        reviewEntity.UserId.Should().Be(review.UserId);
        reviewEntity.BookId.Should().Be(review.BookId);
        reviewEntity.Rating.Should().Be(review.Rating);
        reviewEntity.Comment.Should().Be(review.Comment);
        reviewEntity.CreatedAt.Should().Be(review.CreatedAt);
    }

    [Fact]
    public void ReviewEntity_To_Review_ThrowsForInvalidRating()
    {
        // Arrange
        var invalidReviewEntity = new ReviewEntity
        {
            Rating = 11, // Invalid
            UserId = Guid.NewGuid(),
            BookId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        var ex = Assert.Throws<AutoMapperMappingException>(() =>
            _mapper.Map<Review>(invalidReviewEntity)
        );

        ex.Should().BeOfType<AutoMapperMappingException>();
        ex.Message.Should().Contain("Rating must be between 1 and 10");
    }
}