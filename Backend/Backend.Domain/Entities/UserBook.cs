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

    private UserBook(
        Guid id,
        Guid ownerId,
        Guid generalBookId,
        BookStatus status,
        BookState state,
        LanguageCode language,
        int pageCount)
    {
        Id = id;
        OwnerId = ownerId;
        GeneralBookId = generalBookId;
        Status = status;
        State = state;
        Language = language;
        PageCount = pageCount;
    }

    public static Result<UserBook> Create(
        Guid ownerId,
        Guid generalBookId,
        BookStatus status,
        BookState state,
        string language,
        int pageCount)
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

        var lng = LanguageCode.Create(language);

        if (lng.IsFailed)
            errors.Add(new Error("wrong language code")); // ik this shoudl be propagated by lng.Errors
        
        if (errors.Any())
            return Result.Fail<UserBook>(errors);
        

        return new UserBook(
            Guid.NewGuid(),
            ownerId,
            generalBookId,
            status,
            state,
            lng.Value,
            pageCount
        );
    }

    public Result UpdateStatus(BookStatus newStatus)
    {
        if (!Enum.IsDefined(typeof(BookStatus), newStatus))
            return Result.Fail(UserBookErrors.InvalidStatus);

        Status = newStatus;
        return Result.Ok();
    }
}