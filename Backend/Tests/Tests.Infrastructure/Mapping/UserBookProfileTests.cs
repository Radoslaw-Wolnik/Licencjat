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

public class UserBookProfileTests
{
    private readonly IMapper _mapper;

    public UserBookProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserBookProfile>();
            cfg.AddProfile<BookmarkProfile>();
        });
        
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void UserBookEntity_To_UserBook_MapsCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var entity = new UserBookEntity
        {
            Id = Guid.NewGuid(),
            Language = "pl",
            PageCount = 350,
            CoverPhoto = "https://example.com/cover.jpg",
            Status = BookStatus.Reading,
            State = BookState.Available,
            UserId = userId,
            BookId = bookId,
            Bookmarks = new List<BookmarkEntity>
            {
                new() { Id = Guid.NewGuid(), UserBookId = bookId, Colour = BookmarkColours.yesllow, Page = 42, Description = "Important point" },
                new() { Id = Guid.NewGuid(), UserBookId = bookId, Colour = BookmarkColours.red, Page = 150 }
            }
        };

        // Act
        var userBook = _mapper.Map<UserBook>(entity);

        // Assert
        userBook.Id.Should().Be(entity.Id);
        userBook.OwnerId.Should().Be(entity.UserId);
        userBook.GeneralBookId.Should().Be(entity.BookId);
        userBook.Status.Should().Be(entity.Status);
        userBook.State.Should().Be(entity.State);
        userBook.Language.Code.Should().Be(entity.Language);
        userBook.PageCount.Should().Be(entity.PageCount);
        userBook.CoverPhoto.Link.Should().Be(entity.CoverPhoto);
        userBook.Bookmarks.Should().HaveCount(2);
        userBook.Bookmarks.First().Description.Should().Be("Important point");
    }

    [Fact]
    public void UserBook_To_UserBookEntity_MapsCorrectly()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        // var bookmark = new BookmarkEntity { Id = Guid.NewGuid(), Colour = BookmarkColours.red, Page = 100, Description = "Great chapter", UserBookId = bookId };
        var bookmark = Bookmark.Create(Guid.NewGuid(), bookId, BookmarkColours.red, 47, "Aaaaa");
        var userBook = UserBook.Reconstitute(
            id: bookId,
            ownerId: Guid.NewGuid(),
            generalBookId: Guid.NewGuid(),
            status: BookStatus.Finished,
            state: BookState.Borrowed,
            language: LanguageCode.Create("es").Value,
            pageCount: 420,
            coverPhoto: Photo.Create("https://example.com/spanish-cover.jpg").Value,
            bookmarks: new List<Bookmark>
            {
                bookmark.Value
            }
        ).Value;

        // Act
        var entity = _mapper.Map<UserBookEntity>(userBook);

        // Assert
        entity.Id.Should().Be(userBook.Id);
        entity.UserId.Should().Be(userBook.OwnerId);
        entity.BookId.Should().Be(userBook.GeneralBookId);
        entity.Status.Should().Be(userBook.Status);
        entity.State.Should().Be(userBook.State);
        entity.Language.Should().Be(userBook.Language.Code);
        entity.PageCount.Should().Be(userBook.PageCount);
        entity.CoverPhoto.Should().Be(userBook.CoverPhoto.Link);
        entity.Bookmarks.Should().HaveCount(1);
    }

    [Fact]
    public void UserBookEntity_To_UserBook_HandlesEmptyBookmarks()
    {
        // Arrange
        var entity = new UserBookEntity
        {
            Language = "fr",
            PageCount = 200,
            CoverPhoto = "https://example.com/empty-bookmarks.jpg",
            Status = BookStatus.Waiting,
            State = BookState.Available,
            UserId = Guid.NewGuid(),
            BookId = Guid.NewGuid(),
            Bookmarks = new List<BookmarkEntity>()
        };

        // Act
        var userBook = _mapper.Map<UserBook>(entity);

        // Assert
        userBook.Bookmarks.Should().BeEmpty();
    }

    [Fact]
    public void UserBookEntity_To_UserBook_ThrowsForInvalidLanguage()
    {
        // Arrange
        var entity = new UserBookEntity
        {
            Language = "invalid",  // Invalid language code
            PageCount = 100,
            CoverPhoto = "https://valid.com/cover.jpg",
            Status = BookStatus.Finished,
            State = BookState.Available,
            UserId = Guid.NewGuid(),
            BookId = Guid.NewGuid()
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<UserBook>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*bad language code*");
    }

    [Fact]
    public void UserBookEntity_To_UserBook_ThrowsForInvalidCoverPhoto()
    {
        // Arrange
        var entity = new UserBookEntity
        {
            Language = "de",
            PageCount = 300,
            CoverPhoto = "",  // Invalid empty cover photo
            Status = BookStatus.Reading,
            State = BookState.Available,
            UserId = Guid.NewGuid(),
            BookId = Guid.NewGuid()
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<UserBook>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*somehow photo was null but error was not thrown*"); // cover photo must be initialised during domain create of the object
    }

    [Fact (Skip = "Its not possible to get this scenario")]
    public void UserBookEntity_To_UserBook_ThrowsForInvalidPageCount()
    {
        // Arrange
        var entity = new UserBookEntity
        {
            Language = "ja",
            PageCount = -10,  // Invalid page count
            CoverPhoto = "https://valid.com/jp-cover.jpg",
            Status = BookStatus.Finished,
            State = BookState.Available,
            UserId = Guid.NewGuid(),
            BookId = Guid.NewGuid()
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<UserBook>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*The page count was invalid*");
    }

    [Fact]
    public void UserBook_To_UserBookEntity_IgnoresBookmarksCollection()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var bookmark = Bookmark.Create(Guid.NewGuid(), bookId, BookmarkColours.red, 47, "Aaaaa");
        var userBook = UserBook.Reconstitute(
            id: bookId,
            ownerId: Guid.NewGuid(),
            generalBookId: Guid.NewGuid(),
            status: BookStatus.Finished,
            state: BookState.Available,
            language: LanguageCode.Create("en").Value,
            pageCount: 300,
            coverPhoto: Photo.Create("https://example.com/ignore-bookmarks.jpg").Value,
            bookmarks: new List<Bookmark>
            {
                bookmark.Value
            }
        ).Value;

        // Act
        var entity = _mapper.Map<UserBookEntity>(userBook);

        // Assert
        entity.Bookmarks.Should().HaveCount(1);
    }
}