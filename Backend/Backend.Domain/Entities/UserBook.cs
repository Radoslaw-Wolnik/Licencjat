using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using Backend.Domain.Collections;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class UserBook : Entity<Guid>
{
    public Guid OwnerId { get; }
    public Guid GeneralBookId { get; }
    public BookStatus Status { get; private set; }
    public BookState State { get; private set; }
    public LanguageCode Language { get; }
    public int PageCount { get; }
    public Photo CoverPhoto { get; private set; }

    private readonly BookmarksCollection _bookmarks;
    public IReadOnlyCollection<Bookmark> Bookmarks => _bookmarks.Bookmarks;

    // create 
    private UserBook(
        Guid? id,
        Guid ownerId,
        Guid generalBookId,
        BookStatus status,
        BookState state,
        LanguageCode language,
        int pageCount,
        Photo coverPhoto
    ) : this (
        id?? Guid.NewGuid(), 
        ownerId, 
        generalBookId, 
        status, 
        state, 
        language, 
        pageCount, 
        coverPhoto, 
        initialBookmarks: Enumerable.Empty<Bookmark>()
    ) {}
    
    // reconstituate
    private UserBook(
        Guid id,
        Guid ownerId,
        Guid generalBookId,
        BookStatus status,
        BookState state,
        LanguageCode language,
        int pageCount,
        Photo coverPhoto,
        IEnumerable<Bookmark> initialBookmarks)
    {
        Id = id;
        OwnerId = ownerId;
        GeneralBookId = generalBookId;
        Status = status;
        State = state;
        Language = language;
        PageCount = pageCount;
        CoverPhoto = coverPhoto;
        _bookmarks = new BookmarksCollection(initialBookmarks);
    }

    public static Result<UserBook> Create(
        Guid? id,
        Guid ownerId,
        Guid generalBookId,
        BookStatus status,
        BookState state,
        LanguageCode language,
        int pageCount,
        Photo coverPhoto)
    {
        var errors = new List<IError>();
        
        if (ownerId == Guid.Empty)
            errors.Add(DomainErrorFactory.NotFound("User", ownerId));

        if (generalBookId == Guid.Empty)
            errors.Add(DomainErrorFactory.NotFound("GeneralBook", generalBookId));

        if (!Enum.IsDefined(typeof(BookStatus), status))
            errors.Add(DomainErrorFactory.Invalid("BookStatus", "Given status of userbook was found invalid"));

        if (!Enum.IsDefined(typeof(BookState), state))
            errors.Add(DomainErrorFactory.Invalid("BookState", "Given book state was found invalid"));

        if (pageCount <= 0)
            errors.Add(DomainErrorFactory.Invalid("UserBook", "The page count was invalid"));
        

        if (errors.Count != 0)
            return Result.Fail<UserBook>(errors);

        return new UserBook(
            id,
            ownerId,
            generalBookId,
            status,
            state,
            language,
            pageCount,
            coverPhoto
        );
    }

    public static Result<UserBook> Reconstitute(
        Guid id,
        Guid ownerId,
        Guid generalBookId,
        BookStatus status,
        BookState state,
        LanguageCode language,
        int pageCount,
        Photo coverPhoto,
        IEnumerable<Bookmark> bookmarks
    ) {
         var userBook = new UserBook(
            id,
            ownerId,
            generalBookId,
            status,
            state,
            language,
            pageCount,
            coverPhoto,
            bookmarks
        );
        
        return userBook;
    }

    public void UpdateStatus(BookStatus newStatus)
        => Status = newStatus;

    public void UpdateState(BookState newState)
        => State = newState;

    public void UpdateCover(Photo newCover)
        => CoverPhoto = newCover;
    
    public Result AddBookmark(Bookmark bookmark)
        => _bookmarks.Add(bookmark);
    
    public Result RemoveBookmark(Guid bookmarkId)
        => _bookmarks.Remove(bookmarkId);
    
    public Result UpdateBookmark(Bookmark updatedBookmark)
        => _bookmarks.Update(updatedBookmark);
}