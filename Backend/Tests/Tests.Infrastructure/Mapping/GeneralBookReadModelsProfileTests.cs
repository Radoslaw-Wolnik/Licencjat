using AutoMapper;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Mapping;
using FluentAssertions;

namespace Tests.Infrastructure.Mapping;

public class GeneralBookReadModelsProfileTests
{
    private readonly IMapper _mapper;
    private const int MaxReviews = 5;

    public GeneralBookReadModelsProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GeneralBookReadModelProfile>();
            cfg.AddProfile<UserReadModelProfile>();
        });

        _mapper = config.CreateMapper();
    }

    // fails all - 7 of 7

    [Fact]
    public void GeneralBookEntity_To_GeneralBookListItem_MapsCorrectly()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var entity = new GeneralBookEntity
        {
            Id = bookId,
            Title = "Clean Code",
            Author = "Robert C. Martin",
            Published = new DateOnly(2008, 8, 1),
            Language = "en",
            CoverPhoto = "clean-code.jpg",

            Genres = new List<BookGenre> { BookGenre.Academic, BookGenre.Biography },
            UserBooks = [],
            Reviews = new List<ReviewEntity>
            {
                new() { Id = Guid.NewGuid(), Rating = 9, UserId = Guid.NewGuid(), BookId = bookId, CreatedAt = new DateTime(2008, 8, 1)},
                new() { Id = Guid.NewGuid(), Rating = 10, UserId = Guid.NewGuid(), BookId = bookId, CreatedAt = new DateTime(2008, 7, 1) }
            },

        };

        // Act
        var listItem = _mapper.Map<GeneralBookListItem>(entity);

        // Assert
        listItem.Id.Should().Be(entity.Id);
        listItem.Title.Should().Be(entity.Title);
        listItem.Author.Should().Be(entity.Author);
        listItem.CoverUrl.Should().Be(entity.CoverPhoto);
        listItem.RatingAvg.Should().Be(9.5f);
        listItem.PrimaryGenre.Should().Be(BookGenre.Academic);
        listItem.PublicationDate.Should().Be(entity.Published);
    }

    [Fact]
    public void GeneralBookEntity_To_GeneralBookListItem_HandlesMissingData()
    {
        // Arrange
        var entity = new GeneralBookEntity
        {
            Id = Guid.NewGuid(),
            Title = "Unknown Book",
            CoverPhoto = "unknown.jpg",
            Genres = new List<BookGenre>(),
            Reviews = new List<ReviewEntity>()
        };

        // Act
        var listItem = _mapper.Map<GeneralBookListItem>(entity);

        // Assert
        listItem.Author.Should().BeNull();
        listItem.RatingAvg.Should().Be(0);
        listItem.PrimaryGenre.Should().BeNull();
        listItem.PublicationDate.Should().BeNull();
    }

    [Fact]
    public void GeneralBookEntity_To_GeneralBookDetailsReadModel_MapsCorrectly()
    {
        // Arrange
        var entity = new GeneralBookEntity
        {
            Id = Guid.NewGuid(),
            Title = "Domain-Driven Design",
            Author = "Eric Evans",
            Language = "en",
            CoverPhoto = "ddd.jpg",
            Published = new DateOnly(2003, 8, 30),
            Genres = new List<BookGenre> { BookGenre.Academic, BookGenre.Biography },
            Reviews = new List<ReviewEntity>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Rating = 9,
                    Comment = "Great book",
                    CreatedAt = new DateTime(2023, 1, 15),
                    User = new UserEntity {
                        Id = Guid.NewGuid(),
                        UserName = "user1",
                        ProfilePicture = "avatar1.jpg"
                    }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Rating = 10,
                    Comment = "Must read",
                    CreatedAt = new DateTime(2023, 2, 20),
                    User = new UserEntity {
                        Id = Guid.NewGuid(),
                        UserName = "user2",
                        ProfilePicture = "avatar2.jpg"
                    }
                }
            }
        };

        // Act
        var details = _mapper.Map<GeneralBookDetailsReadModel>(entity, opts =>
            opts.Items["MaxReviews"] = MaxReviews);

        // Assert
        details.Id.Should().Be(entity.Id);
        details.Title.Should().Be(entity.Title);
        details.Author.Should().Be(entity.Author);
        details.Published.Should().Be(entity.Published);
        details.LanguageCode.Should().Be(entity.Language);
        details.CoverPhotoUrl.Should().Be(entity.CoverPhoto);
        details.RatingAvg.Should().Be(9.5);
        details.Genres.Should().ContainInOrder(BookGenre.Academic, BookGenre.Biography);

        details.Reviews.Should().HaveCount(2);
        details.Reviews.First().Rating.Should().Be(10); // Should be ordered by date descending
        details.Reviews.First().Comment.Should().Be("Must read");
        details.Reviews.First().User.Username.Should().Be("user2");
    }

    [Fact]
    public void GeneralBookEntity_To_GeneralBookDetailsReadModel_LimitsReviews()
    {
        // Arrange
        var entity = new GeneralBookEntity
        {
            Reviews = Enumerable.Range(1, 10)
                .Select(i => new ReviewEntity
                {
                    Id = Guid.NewGuid(),
                    Rating = 8,
                    CreatedAt = DateTime.Now.AddDays(-i),
                    User = new UserEntity { UserName = $"user{i}" }
                })
                .ToList()
        };

        // Act
        var details = _mapper.Map<GeneralBookDetailsReadModel>(entity, opts =>
            opts.Items["MaxReviews"] = MaxReviews);

        // Assert
        details.Reviews.Should().HaveCount(MaxReviews);
        details.Reviews.Select(r => r.User.Username)
            .Should().ContainInOrder("user1", "user2", "user3", "user4", "user5");
    }

    [Fact]
    public void ReviewEntity_To_ReviewReadModel_MapsCorrectly()
    {
        // Arrange
        var entity = new ReviewEntity
        {
            Id = Guid.NewGuid(),
            Rating = 8,
            Comment = "Good read",
            CreatedAt = new DateTime(2023, 5, 10),
            User = new UserEntity
            {
                Id = Guid.NewGuid(),
                UserName = "reviewer",
                ProfilePicture = "reviewer.jpg"
            }
        };

        // Act
        var review = _mapper.Map<ReviewReadModel>(entity);

        // Assert
        review.Id.Should().Be(entity.Id);
        review.Rating.Should().Be(entity.Rating);
        review.Comment.Should().Be(entity.Comment);
        review.CreatedAt.Should().Be(entity.CreatedAt);
        review.User.UserId.Should().Be(entity.User.Id);
        review.User.Username.Should().Be("reviewer");
        review.User.ProfilePictureUrl.Should().Be("reviewer.jpg");
    }

    [Fact] // this should be changed to throw an error insted of setting the user to null
    public void ReviewEntity_To_ReviewReadModel_HandlesMissingUser()
    {
        // Arrange
        var entity = new ReviewEntity
        {
            Rating = 7,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var review = _mapper.Map<ReviewReadModel>(entity);

        // Assert
        review.User.Should().BeNull();
    }

    [Fact (Skip ="This is how the review should behave but i need to deeply rewrite its mapper - as i cant throw with tenary expressions")]
    public void ReviewEntity_To_ReviewReadModel_Throws_WhenUserMissing()
    {
        // Arrange
        var entity = new ReviewEntity
        {
            Rating    = 7,
            CreatedAt = DateTime.UtcNow
            // entity.User is left null
        };

        // Act
        Action act = () => _mapper.Map<ReviewReadModel>(entity);

        // Assert
        act.Should()
        .Throw<AutoMapperMappingException>()
        .WithMessage("*Review has no User*");
    }

    [Fact]
    public void ReviewEntity_To_ReviewReadModel_HandlesNullComment()
    {
        // Arrange
        var entity = new ReviewEntity
        {
            Rating = 9,
            Comment = null,
            CreatedAt = DateTime.UtcNow,
            User = new UserEntity
            {
                Id = Guid.NewGuid(),
                UserName = "user2",
                ProfilePicture = "avatar2.jpg",
                Reputation = (float)7.8,
                City = "New York",
                Country = "US",
                SubSwaps = []
            }
        };

        // Act
        var review = _mapper.Map<ReviewReadModel>(entity);

        // Assert
        review.Comment.Should().BeNull();
    }
    
    [Fact]
    public void GeneralBookEntity_To_BookCoverItemReadModel_MapsCorrectly()
    {
        // Arrange
        var entity = new GeneralBookEntity
        {
            Id = Guid.NewGuid(),
            Title = "Cover Book",
            CoverPhoto = "cover-item.jpg"
        };

        // Act
        var coverModel = _mapper.Map<BookCoverItemReadModel>(entity);

        // Assert
        coverModel.Id.Should().Be(entity.Id);
        coverModel.Title.Should().Be("Cover Book");
        coverModel.CoverUrl.Should().Be("cover-item.jpg");
    }

}