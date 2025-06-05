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

public class GeneralBookProfileTests
{
    private readonly IMapper _mapper;

    public GeneralBookProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GeneralBookProfile>();
            cfg.AddProfile<ReviewProfile>();
            cfg.AddProfile<UserBookProfile>();
        });
        
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void GeneralBookEntity_To_GeneralBook_MapsCorrectly()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var userBook = new UserBookEntity { Id = Guid.NewGuid(), Language = "en", PageCount = 200, UserId = Guid.NewGuid(), BookId = bookId, CoverPhoto = "linktocover.jpg" };
        var entity = new GeneralBookEntity
        {
            Id = bookId,
            Title = "Domain-Driven Design",
            Author = "Eric Evans",
            Published = new DateOnly(2003, 8, 30),
            Language = "en",
            CoverPhoto = "dddcovers.com/ddd.jpg",
            Genres = new List<BookGenre> { BookGenre.ScienceFiction, BookGenre.Academic },
            UserBooks = new List<UserBookEntity>
            {
                userBook, userBook
            },
            Reviews = new List<ReviewEntity>
            {
                new() { Id = Guid.NewGuid(), Rating = 9, UserId = Guid.NewGuid(), BookId = bookId},
                new() { Id = Guid.NewGuid(), Rating = 10, UserId = Guid.NewGuid(), BookId = bookId }
            }
        };

        // Act
        var book = _mapper.Map<GeneralBook>(entity);

        // Assert
        book.Id.Should().Be(entity.Id);
        book.Title.Should().Be(entity.Title);
        book.Author.Should().Be(entity.Author);
        book.Published.Should().Be(entity.Published);
        book.OriginalLanguage.Code.Should().Be(entity.Language);
        book.CoverPhoto.Link.Should().Be(entity.CoverPhoto);
        book.Genres.Should().BeEquivalentTo(entity.Genres);
        book.RatingAvg.Value.Should().Be(9.5f);
        book.UserCopies.Should().HaveCount(2);
        book.UserReviews.Should().HaveCount(2);
    }

    [Fact]
    public void GeneralBook_To_GeneralBookEntity_MapsCorrectly()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = GeneralBook.Reconstitute(
            id: bookId,
            title: "Clean Code",
            author: "Robert C. Martin",
            published: new DateOnly(2008, 8, 1),
            originalLanguage: LanguageCode.Create("en").Value,
            coverPhoto: Photo.Create("cleancodecovers.com/clean.jpg").Value,
            ratingAvg: Rating.Create(9.2f).Value,
            genres: new[] { BookGenre.Drama, BookGenre.Children },
            userCopies: new List<UserBook>
            {
                UserBook.Create(Guid.NewGuid(), Guid.NewGuid(), bookId, BookStatus.Finished, BookState.Available, LanguageCode.Create("en").Value, 200, new Photo("linkkkk")).Value
            },
            userReviews: new List<Review>
            {
                Review.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 9, DateTime.UtcNow, null).Value
            }
        );

        // Act
        var entity = _mapper.Map<GeneralBookEntity>(book);

        // Assert
        entity.Id.Should().Be(book.Id);
        entity.Title.Should().Be(book.Title);
        entity.Author.Should().Be(book.Author);
        entity.Published.Should().Be(book.Published);
        entity.Language.Should().Be(book.OriginalLanguage.Code);
        entity.CoverPhoto.Should().Be(book.CoverPhoto.Link);
        entity.Genres.Should().BeEquivalentTo(book.Genres);
        entity.UserBooks.Should().BeEmpty(); // Should be ignored
        entity.Reviews.Should().HaveCount(1);  
    }

    [Fact]
    public void GeneralBookEntity_To_GeneralBook_HandlesEmptyCollections()
    {
        // Arrange
        var entity = new GeneralBookEntity
        {
            Id = Guid.NewGuid(),
            Title = "Empty Collections Test",
            Author = "Test Author",
            Published = new DateOnly(2020, 1, 1),
            Language = "pl",
            CoverPhoto = "testcover.com/test.jpg",
            Genres = [],
            UserBooks = [],
            Reviews = []
        };

        // Act
        var book = _mapper.Map<GeneralBook>(entity);

        // Assert
        book.Genres.Should().BeEmpty();
        book.UserCopies.Should().BeEmpty();
        book.UserReviews.Should().BeEmpty();
        book.RatingAvg.Value.Should().Be(7f);
    }

    [Fact]
    public void GeneralBookEntity_To_GeneralBook_ThrowsForInvalidLanguage()
    {
        // Arrange
        var invalidEntity = new GeneralBookEntity
        {
            Language = "invalid-language-code",
            Title = "Test",
            Author = "Author",
            Published = DateOnly.MinValue,
            CoverPhoto = "cover.com/photo.jpg"
        };

        // Act & Assert
        var ex = Assert.Throws<AutoMapperMappingException>(() => 
            _mapper.Map<GeneralBook>(invalidEntity)
        );
        
        ex.Should().BeOfType<AutoMapperMappingException>();
        ex.Message.Should().Contain("Invalid country code");
    }

    [Fact]
    public void GeneralBookEntity_To_GeneralBook_ThrowsForInvalidCoverPhoto()
    {
        // Arrange
        var invalidEntity = new GeneralBookEntity
        {
            Language = "en",
            Title = "Test",
            Author = "Author",
            Published = DateOnly.MinValue,
            CoverPhoto = "" // Invalid
        };

        // Act & Assert
        var ex = Assert.Throws<AutoMapperMappingException>(() =>
            _mapper.Map<GeneralBook>(invalidEntity)
        );


        ex.Message.Should().Contain("Invalid empty link for photo");
        ex.Should().BeOfType<AutoMapperMappingException>();
    }

    [Fact]
    public void GeneralBookEntity_To_GeneralBook_CalculatesAverageRatingCorrectly()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var entity = new GeneralBookEntity
        {
            Language = "en",
            Title = "Rating Test",
            Author = "Author",
            Published = DateOnly.MinValue,
            CoverPhoto = "cover.com/photo.jpg",
            Reviews = new List<ReviewEntity>
            {
                new() { Id = Guid.NewGuid(), Rating = 5, UserId = Guid.NewGuid(), BookId = bookId },
                new() { Id = Guid.NewGuid(), Rating = 7, UserId = Guid.NewGuid(), BookId = bookId },
                new() { Id = Guid.NewGuid(), Rating = 9, UserId = Guid.NewGuid(), BookId = bookId }
            }
        };

        // Act
        var book = _mapper.Map<GeneralBook>(entity);

        // Assert
        book.RatingAvg.Value.Should().Be(7f); // (5+7+9)/3 = 7
    }

    [Fact]
    public void GeneralBook_To_GeneralBookEntity_DoesNotMapRelations()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = GeneralBook.Reconstitute(
            id: bookId,
            title: "Relations Test",
            author: "Author",
            published: new DateOnly(2020, 1, 1),
            originalLanguage: LanguageCode.Create("en").Value,
            coverPhoto: Photo.Create("cover.com/photo.jpg").Value,
            ratingAvg: Rating.Create(8.5f).Value,
            genres: new[] { BookGenre.Fiction },
            userCopies: new List<UserBook>
            {
                UserBook.Create(Guid.NewGuid(), Guid.NewGuid(), bookId, BookStatus.Finished, BookState.Available, LanguageCode.Create("en").Value, 200, new Photo("linkkkk")).Value
            },
            userReviews: new List<Review>
            {
                Review.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10, DateTime.UtcNow, "Amazing").Value
            }
        );

        // Act
        var entity = _mapper.Map<GeneralBookEntity>(book);

        // Assert
        entity.UserBooks.Should().BeEmpty();
        entity.Reviews.Should().HaveCount(1);
        entity.WishlistedByUsers.Should().BeEmpty();
    }

    [Fact]
    public void GeneralBookEntity_To_GeneralBook_UsesValueObjectsCorrectly()
    {
        // Arrange
        var entity = new GeneralBookEntity
        {
            Language = "fr",
            CoverPhoto = "https://example.com/cover.jpg",
            Title = "Value Objects",
            Author = "Author",
            Published = new DateOnly(2023, 1, 1)
        };

        // Act
        var book = _mapper.Map<GeneralBook>(entity);

        // Assert
        book.OriginalLanguage.Should().BeOfType<LanguageCode>();
        book.OriginalLanguage.Code.Should().Be("fr");
        book.CoverPhoto.Should().BeOfType<Photo>();
        book.CoverPhoto.Link.Should().Be("https://example.com/cover.jpg");
    }
}