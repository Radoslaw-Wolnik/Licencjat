using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using Backend.Domain.ValueObjects;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class GeneralBook : Entity<Guid>
{
    public string Title { get; private set; }
    public string Author { get; private set; }
    public DateOnly Published { get; private set; }
    public LanguageCode OriginalLanguage { get; private set; }
    public Rating RatingAvg { get; private set; } = Rating.Initial();
    public Photo CoverPhoto { get; private set; }
    
    private readonly GenresCollection _genres;
    public IReadOnlyCollection<BookGenre> Genres => _genres.Genres;
    
    private readonly UserCopiesCollection _userCopies;
    public IReadOnlyCollection<UserBook> UserCopies => _userCopies.UserBooks;

    private readonly ReviewsCollection _reviews;
    public IReadOnlyCollection<Review> UserReviews => _reviews.Reviews;

    // constructor for Create — empty collections
    private GeneralBook(
        Guid? id,
        string title,
        string author,
        DateOnly published,
        LanguageCode originalLanguage,
        Photo coverPhoto)
     : this (
        id?? Guid.NewGuid(), 
        title, author, published, originalLanguage, coverPhoto,
        initialGenres: Enumerable.Empty<BookGenre>(),
        initialUserCopies: [],
        initialReviews: []
    ) {}

    // reconstitution constructor — bulk‑load from persistence
    private GeneralBook(
        Guid id,
        string title,
        string author,
        DateOnly published,
        LanguageCode originalLanguage,
        Photo coverPhoto,
        IEnumerable<BookGenre> initialGenres,
        IEnumerable<UserBook> initialUserCopies,
        IEnumerable<Review> initialReviews)
    {
        Id = id;
        Title = title;
        Author = author;
        Published = published;
        OriginalLanguage = originalLanguage;
        CoverPhoto = coverPhoto;
        _genres = new GenresCollection(initialGenres);
        _userCopies = new UserCopiesCollection(initialUserCopies); 
        _reviews = new ReviewsCollection (initialReviews);
    }

    public static Result<GeneralBook> Create(
        Guid? id,
        string title,
        string author,
        DateOnly published,
        LanguageCode originalLanguage,
        Photo coverPhoto)
    {
        var errors = new List<IError>();
        
        if (errors.Count != 0)
            return Result.Fail<GeneralBook>(errors);

        return new GeneralBook(
            id,
            title,
            author,
            published,
            originalLanguage,
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
        IEnumerable<UserBook> userCopies,
        IEnumerable<Review> userReviews
    )
    {
        var book =  new GeneralBook(id, title, author, published, originalLanguage, coverPhoto, genres, userCopies, userReviews)
        {
            RatingAvg = ratingAvg,
        };

        return book;
    }

    public Result UpdateCoverPhoto(Photo photo){
        CoverPhoto = photo;
        return Result.Ok();
    }

    public Result UpdateScalarValues(string? title, string? author, DateOnly? published, LanguageCode? language)
    {
        // update not null values
        Title = title ?? Title;
        Author = author ?? Author;
        Published = published ?? Published;
        OriginalLanguage = language ?? OriginalLanguage;

        return Result.Ok();
    }

    public void UpdateRating(Rating newRating)
        => RatingAvg = newRating;
    
    public Result ReplaceGenres(IEnumerable<BookGenre> bookGenres)
        => _genres.Replace(bookGenres);

    public Result AddGenre(BookGenre genre)
        => _genres.Add(genre);
    
    public Result RemoveGenre(BookGenre genre)
        => _genres.Remove(genre);

    public Result AddUserCopy(UserBook userBook)
        => _userCopies.Add(userBook);
    
    public Result RemoveUserCopy(Guid userBookId)
        => _userCopies.Remove(userBookId);
    
    public Result UpdateUserCopy(UserBook userBook)
        => _userCopies.Update(userBook);

    public Result AddReview(Review review)
        => _reviews.Add(review);
    
    public Result RemoveReview(Guid reviewId)
        =>  _reviews.Remove(reviewId);
    
    public Result UpdateReview(Review review)
        => _reviews.Update(review);
}