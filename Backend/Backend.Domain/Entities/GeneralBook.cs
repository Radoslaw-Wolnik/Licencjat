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
    public Rating RatingAvg { get; private set; } // 1-10 
    public Photo CoverPhoto { get; }
    
    private readonly List<BookGenre> _genres = new();
    public IReadOnlyCollection<BookGenre> Genres => _genres.AsReadOnly();
    
    private readonly List<Guid> _userCopies = new();
    public IReadOnlyCollection<Guid> UserCopies => _userCopies.AsReadOnly();

    private readonly List<Guid> _userReviews = new();
    public IReadOnlyCollection<Guid> UserReviews => _userReviews.AsReadOnly();

    private GeneralBook(
        Guid id,
        string title,
        string author,
        DateOnly published,
        LanguageCode originalLanguage,
        Rating ratingAvg,
        Photo coverPhoto)
    {
        Id = id;
        Title = title;
        Author = author;
        Published = published;
        OriginalLanguage = originalLanguage;
        RatingAvg = ratingAvg;
        CoverPhoto = coverPhoto;
    }

    public static Result<GeneralBook> Create(
        string title,
        string author,
        DateOnly published,
        LanguageCode originalLanguage,
        Rating ratingAvg,
        Photo coverPhoto)
    {
        var errors = new List<IError>();
        
        if (errors.Any())
            return Result.Fail<GeneralBook>(errors);

        return new GeneralBook(
            Guid.NewGuid(),
            title,
            author,
            published,
            originalLanguage,
            ratingAvg,
            coverPhoto
        );
    }

    public static GeneralBook Reconstitute(
        Guid id,
        string title,
        string author,
        DateOnly published,
        LanguageCode originalLanguage,
        Photo coverPhoto,
        Rating ratingAvg,
        IEnumerable<BookGenre> genres,
        IEnumerable<Guid> userCopies,
        IEnumerable<Guid> userReviews
    )
    {
        var book =  new GeneralBook(id, title, author, published, originalLanguage, ratingAvg, coverPhoto);

        foreach (var g in genres)      book._genres.Add(g);
        foreach (var c in userCopies)  book._userCopies.Add(c);
        foreach (var r in userReviews) book._userReviews.Add(r);

        return book;
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


    public Result AddUserReview(Guid reviewId, int reviewRating)
    {

        if (_userReviews.Contains(reviewId))
            return Result.Fail(ReviewErrors.Duplicate);
        
        _userReviews.Add(reviewId);

        var countOld = _userReviews.Count - 1; // before adding
        var totalOld = RatingAvg.Value * countOld;
        var newAvg = (totalOld + reviewRating) / _userReviews.Count;
        RatingAvg = Rating.Create(newAvg).Value;
        return Result.Ok();
    }


}