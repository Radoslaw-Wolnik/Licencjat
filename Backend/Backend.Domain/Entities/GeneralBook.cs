using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using Backend.Domain.ValueObjects;
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
    
    private readonly GenresCollection _genres = new();
    public IReadOnlyCollection<BookGenre> Genres => _genres.Genres;
    
    private readonly UserCopiesCollection _userCopies = new();
    public IReadOnlyCollection<UserBook> UserCopies => _userCopies.UserBooks;

    private readonly ReviewsCollection _reviews = new();
    public IReadOnlyCollection<Review> UserReviews => _reviews.Reviews;

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
        Guid id,
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
            Rating.Initial(),
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
        var book =  new GeneralBook(id, title, author, published, originalLanguage, ratingAvg, coverPhoto);

        foreach (var g in genres)      book._genres.Add(g);
        foreach (var c in userCopies)  book._userCopies.Add(c);
        foreach (var r in userReviews) book._reviews.Add(r);

        return book;
    }


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