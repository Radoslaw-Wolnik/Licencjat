// Backend.Domain/Entities/UserBook.cs
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
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

    private List<Bookmark> _bookmarks = new();
    public IReadOnlyCollection<Bookmark> Bookmarks => _bookmarks.AsReadOnly();


    private UserBook(
        Guid id,
        Guid ownerId,
        Guid generalBookId,
        BookStatus status,
        BookState state,
        LanguageCode language,
        int pageCount,
        Photo coverPhoto)
    {
        Id = id;
        OwnerId = ownerId;
        GeneralBookId = generalBookId;
        Status = status;
        State = state;
        Language = language;
        PageCount = pageCount;
        CoverPhoto = coverPhoto;
    }

    public static Result<UserBook> Create(
        Guid id,
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
            errors.Add(UserErrors.NotFound);

        if (generalBookId == Guid.Empty)
            errors.Add(BookErrors.NotFound);

        if (!Enum.IsDefined(typeof(BookStatus), status))
            errors.Add(UserBookErrors.InvalidStatus);

        if (!Enum.IsDefined(typeof(BookState), state))
            errors.Add(UserBookErrors.InvalidState);

        if (pageCount <= 0)
            errors.Add(UserBookErrors.InvalidPageCount);
        

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
            coverPhoto
        );

        foreach (var b in bookmarks) userBook._bookmarks.Add(b);
        
        return userBook;
    }

    public Result UpdateStatus(BookStatus newStatus)
    {
        if (!Enum.IsDefined(typeof(BookStatus), newStatus))
            return Result.Fail(UserBookErrors.InvalidStatus);

        Status = newStatus;
        return Result.Ok();
    }
}