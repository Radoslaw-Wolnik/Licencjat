using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Domain.Common;
using Backend.Domain.Errors;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FluentResults;
using FluentAssertions;
using Tests.Infrastructure.Helpers;
using Backend.Infrastructure.Mapping;

namespace Tests.Infrastructure.Repositories;

public class WriteGeneralBookRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly WriteGeneralBookRepository _repository;

    public WriteGeneralBookRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        
        var config = new MapperConfiguration(cfg => 
        {
            cfg.AddProfile<GeneralBookProfile>();
            cfg.AddProfile<ReviewProfile>();
        });
        _mapper = config.CreateMapper();

        

        _repository = new WriteGeneralBookRepository(_context, _mapper);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task AddAsync_WithValidBook_ReturnsGuid()
    {
        // Arrange
        var book = CreateTestBook();
        // book.Should().NotBeEmpty();

        // domainError.Type.Should().Be(ErrorType.Validation);

        // Act
        var result = await _repository.AddAsync(book, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        (await _context.GeneralBooks.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task DeleteAsync_ExistingBook_DeletesSuccessfully()
    {
        // Arrange
        var book = CreateTestBook();
        var bookId = (await _repository.AddAsync(book, CancellationToken.None)).Value;

        // Act
        var result = await _repository.DeleteAsync(bookId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await _context.GeneralBooks.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingBook_ReturnsNotFoundError()
    {
        // Act
        var result = await _repository.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeNotFoundError("GeneralBook");
    }

    [Fact]
    public async Task UpdateScalarsAsync_ExistingBook_UpdatesProperties()
    {
        // Arrange
        var book = CreateTestBook();
        var bookId = (await _repository.AddAsync(book, CancellationToken.None)).Value;
        var updatedBook = GeneralBook.Reconstitute(
            bookId,
            "Updated Title",
            "Updated Author",
            new DateOnly(2022, 1, 1),
            LanguageCode.Create("fr").ValueOrDefault,
            new Photo("updated.jpg"),
            new Rating(4.8f),
            new[] { BookGenre.Mystery },
            Enumerable.Empty<UserBook>(),
            Enumerable.Empty<Review>()
        );

        // Act
        var result = await _repository.UpdateScalarsAsync(updatedBook, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dbBook = await _context.GeneralBooks.FirstAsync();
        dbBook.Title.Should().Be("Updated Title");
        dbBook.Author.Should().Be("Updated Author");
        dbBook.Published.Should().Be(new DateOnly(2022, 1, 1));
        dbBook.Language.Should().Be("fr");
        dbBook.CoverPhoto.Should().Be("updated.jpg");
    }

    [Fact]
    public async Task UpdateScalarsAsync_NonExistingBook_ReturnsNotFound()
    {
        // Arrange
        var nonExistingBook = CreateTestBook();

        // Act
        var result = await _repository.UpdateScalarsAsync(nonExistingBook, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        var domainErr = (DomainError)result.Errors[0];
        domainErr.Type.Should().Be(ErrorType.StorageError);
    }

    [Fact]
    public async Task AddReviewAsync_ValidReview_AddsSuccessfully()
    {
        // Arrange
        var book = CreateTestBook();
        var bookId = (await _repository.AddAsync(book, CancellationToken.None)).Value;
        var review = Review.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            bookId,
            4,
            DateTime.Now,
            "Great book!"
        ).Value;

        // Act
        var result = await _repository.AddReviewAsync(bookId, review, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dbBook = await _context.GeneralBooks
            .Include(b => b.Reviews)
            .FirstAsync(b => b.Id == bookId);
        dbBook.Reviews.Should().ContainSingle();
    }

    [Fact]
    public async Task AddReviewAsync_NonExistingBook_ReturnsNotFound()
    {
        // Arrange
        var review = Review.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            4,
            DateTime.Now,
            "Great book!"
        ).Value;

        // Act
        var result = await _repository.AddReviewAsync(Guid.NewGuid(), review, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeNotFoundError("GeneralBook");
    }

    [Fact]
    public async Task UpdateReviewAsync_ExistingReview_UpdatesProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var review = Review.Create(
            Guid.NewGuid(),
            userId,
            bookId,
            4,
            DateTime.Now,
            "Good book"
        ).Value;
        
        var book = CreateTestBook(id: bookId, reviews: new[] { review });
        await _repository.AddAsync(book, CancellationToken.None);
        
        var updatedReview = Review.Create(
            review.Id,
            userId,
            bookId,
            5,
            DateTime.Now,
            "Excellent!"
        ).Value;

        // Act
        var result = await _repository.UpdateReviewAsync(updatedReview, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dbBook = await _context.GeneralBooks.FirstAsync();
        dbBook.Reviews.First().Rating.Should().Be(5);
        dbBook.Reviews.First().Comment.Should().Be("Excellent!");
    }

    [Fact]
    public async Task UpdateReviewAsync_NonExistingReview_ReturnsNotFound()
    {
        // Arrange
        var review = Review.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            4,
            DateTime.Now,
            "Good book"
        ).Value;

        // Act
        var result = await _repository.UpdateReviewAsync(review, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeNotFoundError("Review");
    }

    [Fact]
    public async Task RemoveReviewAsync_ExistingReview_RemovesSuccessfully()
    {
        // Arrange
        var review = Review.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            4,
            DateTime.Now,
            "Good book"
        ).Value;
        
        var book = CreateTestBook(reviews: new[] { review });
        await _repository.AddAsync(book, CancellationToken.None);

        // Act
        var result = await _repository.RemoveReviewAsync(review.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Verify through book aggregate
        var dbBook = await _context.GeneralBooks
            .Include(b => b.Reviews)
            .FirstAsync();
        dbBook.Reviews.Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveReviewAsync_NonExistingReview_ReturnsNotFound()
    {
        // Act
        var result = await _repository.RemoveReviewAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeNotFoundError("Review");
    }

    private GeneralBook CreateTestBook(
        Guid? id = null,
        IEnumerable<Review>? reviews = null)
    {
        return GeneralBook.Reconstitute(
            id ?? Guid.NewGuid(),
            "Test Book",
            "Test Author",
            new DateOnly(2020, 1, 1),
            LanguageCode.Create("en").ValueOrDefault,
            new Photo("cover.jpg"),
            new Rating(4.5f),
            new[] { BookGenre.Fiction },
            Enumerable.Empty<UserBook>(),
            reviews ?? Enumerable.Empty<Review>()
        );
    }
}