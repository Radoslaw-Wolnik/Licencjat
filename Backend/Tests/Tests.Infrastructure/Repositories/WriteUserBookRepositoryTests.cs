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

public class WriteUserBookRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly WriteUserBookRepository _repository;

    public WriteUserBookRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        
        var config = new MapperConfiguration(cfg => 
        {
            cfg.AddProfile<UserBookProfile>();
            cfg.AddProfile<BookmarkProfile>();
        });
        _mapper = config.CreateMapper();

        _repository = new WriteUserBookRepository(_context, _mapper);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task AddAsync_WithValidBook_ReturnsGuid()
    {
        // Arrange
        var book = CreateTestBook();

        // Act
        var result = await _repository.AddAsync(book, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        (await _context.UserBooks.CountAsync()).Should().Be(1);
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
        (await _context.UserBooks.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingBook_ReturnsNotFoundError()
    {
        // Act
        var result = await _repository.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeNotFoundError("UserBook");
    }

    [Fact]
    public async Task UpdateScalarsAsync_ExistingBook_UpdatesProperties()
    {
        // Arrange
        var book = CreateTestBook();
        var bookId = (await _repository.AddAsync(book, CancellationToken.None)).Value;
        var updatedBook = UserBook.Reconstitute(
            bookId,
            book.OwnerId,
            book.GeneralBookId,
            BookStatus.Reading,
            BookState.Borrowed,
            LanguageCode.Create("fr").ValueOrDefault,
            book.PageCount,
            new Photo("updated.jpg"),
            book.Bookmarks
        );

        // Act
        var result = await _repository.UpdateScalarsAsync(updatedBook.Value, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dbBook = await _context.UserBooks.FirstAsync();
        dbBook.Status.Should().Be(BookStatus.Reading);
        dbBook.State.Should().Be(BookState.Borrowed);
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
    public async Task UpdateBookmarksAsync_ExistingBook_SyncsBookmarks()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var bookmark1 = Bookmark.Create(
            Guid.NewGuid(), bookId, BookmarkColours.red, 10, "Note 1").Value;
        var bookmark2 = Bookmark.Create(
            Guid.NewGuid(), bookId, BookmarkColours.blue, 20, "Note 2").Value;
        
        var book = CreateTestBook(id: bookId, bookmarks: new[] { bookmark1 });
        await _repository.AddAsync(book, CancellationToken.None);
        
        var updatedBookmarks = new[] 
        {
            Bookmark.Create(bookmark1.Id, bookmark1.UserBookId, BookmarkColours.green, 15, "Updated").Value,
            Bookmark.Create(Guid.NewGuid(), bookId, BookmarkColours.yesllow, 30, "New").Value
        };

        // Act
        var result = await _repository.UpdateBookmarksAsync(bookId, updatedBookmarks, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var dbBook = await _context.UserBooks
            .Include(b => b.Bookmarks)
            .FirstAsync(b => b.Id == bookId);
        
        dbBook.Bookmarks.Should().HaveCount(2);
        dbBook.Bookmarks.Should().Contain(b => 
            b.Id == bookmark1.Id && 
            b.Colour == BookmarkColours.green &&
            b.Page == 15 &&
            b.Description == "Updated");
        dbBook.Bookmarks.Should().Contain(b => b.Description == "New");
    }

    [Fact]
    public async Task UpdateBookmarksAsync_NonExistingBook_ReturnsNotFound()
    {
        // Act
        var result = await _repository.UpdateBookmarksAsync(
            Guid.NewGuid(), 
            Enumerable.Empty<Bookmark>(), 
            CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeNotFoundError("UserBook");
    }

    [Fact]
    public async Task AddBookmarkAsync_ValidBookmark_AddsSuccessfully()
    {
        // Arrange
        var book = CreateTestBook();
        var bookId = (await _repository.AddAsync(book, CancellationToken.None)).Value;
        var bookmark = Bookmark.Create(
            Guid.NewGuid(),
            bookId,
            BookmarkColours.red,
            10,
            "Test Bookmark"
        ).Value;

        // Act
        var result = await _repository.AddBookmarkAsync(bookmark, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var dbBook = await _context.UserBooks
            .Include(b => b.Bookmarks)
            .FirstAsync(b => b.Id == bookId);
        dbBook.Bookmarks.Should().ContainSingle();
    }

    [Fact]
    public async Task RemoveBookmarkAsync_ExistingBookmark_RemovesSuccessfully()
    {
        // Arrange
        var bookmark = Bookmark.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            BookmarkColours.red,
            10,
            "Test Bookmark"
        ).Value;
        
        var book = CreateTestBook(bookmarks: new[] { bookmark });
        await _repository.AddAsync(book, CancellationToken.None);

        // Act
        var result = await _repository.RemoveBookmarkAsync(bookmark.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var dbBook = await _context.UserBooks
            .Include(b => b.Bookmarks)
            .FirstAsync();
        dbBook.Bookmarks.Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveBookmarkAsync_NonExistingBookmark_ReturnsNotFound()
    {
        // Act
        var result = await _repository.RemoveBookmarkAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeNotFoundError("Bookmark");
    }

    [Fact]
    public async Task UpdateBookmarkAsync_ExistingBookmark_UpdatesProperties()
    {
        // Arrange
        var bookmark = Bookmark.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            BookmarkColours.red,
            10,
            "Original"
        ).Value;
        
        var book = CreateTestBook(bookmarks: new[] { bookmark });
        await _repository.AddAsync(book, CancellationToken.None);
        
        var updatedBookmark = Bookmark.Create(
            bookmark.Id,
            bookmark.UserBookId,
            BookmarkColours.blue,
            20,
            "Updated"
        ).Value;

        // Act
        var result = await _repository.UpdateBookmarkAsync(updatedBookmark, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var dbBook = await _context.UserBooks
            .Include(b => b.Bookmarks)
            .FirstAsync();
        var dbBookmark = dbBook.Bookmarks.First();
        dbBookmark.Colour.Should().Be(BookmarkColours.blue);
        dbBookmark.Page.Should().Be(20);
        dbBookmark.Description.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateBookmarkAsync_NonExistingBookmark_ReturnsNotFound()
    {
        // Arrange
        var bookmark = Bookmark.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            BookmarkColours.red,
            10,
            "Test"
        ).Value;

        // Act
        var result = await _repository.UpdateBookmarkAsync(bookmark, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        var domainErr = (DomainError)result.Errors[0];
        domainErr.Type.Should().Be(ErrorType.StorageError);
    }

    private UserBook CreateTestBook(
        Guid? id = null,
        IEnumerable<Bookmark>? bookmarks = null)
    {
        return UserBook.Reconstitute(
            id ?? Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            BookStatus.Finished,
            BookState.Available,
            LanguageCode.Create("en").ValueOrDefault,
            300,
            new Photo("cover.jpg"),
            bookmarks ?? Enumerable.Empty<Bookmark>()
        ).Value;
    }
}