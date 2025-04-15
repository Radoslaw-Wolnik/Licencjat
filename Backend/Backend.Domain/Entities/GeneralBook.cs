// Backend.Domain/Entities/GeneralBook.cs
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class GeneralBook : Entity<Guid>
{
    public string Title { get; }
    public string Author { get; }
    public DateOnly Published { get; }
    public LanguageCode OriginalLanguage { get; }

    private readonly List<Guid> _userCopies = new();
    public IReadOnlyCollection<Guid> UserCopies => _userCopies.AsReadOnly();

    private readonly List<BookGenre> _genres = new();
    
    public IReadOnlyCollection<BookGenre> Genres => _genres.AsReadOnly();
    private readonly List<Guid> _userReviews = new();
    public IReadOnlyCollection<Guid> UserReviews => _userReviews.AsReadOnly();

    private GeneralBook(
        Guid id,
        string title,
        string author,
        DateOnly published,
        LanguageCode originalLanguage)
    {
        Id = id;
        Title = title;
        Author = author;
        Published = published;
        OriginalLanguage = originalLanguage;
    }

    public static Result<GeneralBook> Create(
        string title,
        string author,
        DateOnly published,
        LanguageCode originalLanguage)
    {
        var errors = new List<IError>();

        // if (published > DateTime.UtcNow)
        //     errors.Add(BookErrors.InvalidPublicationDate);

        // honestly we cound check if language is valid here
        // var lng = LanguageCode.Create(originalLanguage);
        // if (lng.IsFailed)
        //     errors.Add(new Error("wrong languge code"));
        // this is a validation of request so it resides in the apliction layer, not here
        
        if (errors.Any())
            return Result.Fail<GeneralBook>(errors);

        return new GeneralBook(
            Guid.NewGuid(),
            title,
            author,
            published,
            originalLanguage
        );
    }

    public Result AddGenre(BookGenre genre)
    {
        if (!Enum.IsDefined(typeof(BookGenre), genre))
            return Result.Fail(BookErrors.InvalidGenre);
            
        if (_genres.Contains(genre))
            return Result.Fail(BookErrors.DuplicateGenre);

        _genres.Add(genre);
        return Result.Ok();
    }

    public Result AddUserCopy(Guid userBookId)
    {
        if (_userCopies.Contains(userBookId))
            return Result.Fail(BookErrors.DuplicateCopy);
            
        _userCopies.Add(userBookId);
        return Result.Ok();
    }

    public Result AddUserReview(Guid reviewId)
    {
        if (_userReviews.Contains(reviewId))
            return Result.Fail(ReviewErrors.Duplicate);
            
        _userReviews.Add(reviewId);
        return Result.Ok();
    }

}