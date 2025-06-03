using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record Bookmark(Guid Id, Guid UserBookId, BookmarkColours Colour, int Page, string? Description)
{
    public static Result<Bookmark> Create(Guid id, Guid userBookId, BookmarkColours colour, int page, string? description)
    {
        var errors = new List<IError>();
        
        if (userBookId == Guid.Empty) errors.Add(DomainErrorFactory.NotFound("UserBook", userBookId));
        if (!Enum.IsDefined(typeof(BookmarkColours), colour)) errors.Add(DomainErrorFactory.Invalid("Bookmark", "Wrong colour"));
        if ( page <= 0 ) errors.Add(DomainErrorFactory.Invalid("Bookmark", "Page must be above 0"));

        if (errors.Count != 0) return Result.Fail<Bookmark>(errors);

        return new Bookmark(
            id,
            userBookId,
            colour,
            page,
            description
        );
    }
}