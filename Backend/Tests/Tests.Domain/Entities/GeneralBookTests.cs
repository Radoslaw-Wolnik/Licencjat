using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentAssertions;

namespace Tests.Domain.Entities;

public class GeneralBookTests
{
    private readonly Guid _id = Guid.NewGuid();
    private const string Title = "Test Book";
    private const string Author = "Test Author";
    private readonly DateOnly _published = new(2020, 1, 1);
    private LanguageCode Language = LanguageCode.Create("en").ValueOrDefault;
    private readonly Photo _coverPhoto = new("cover.jpg");

    [Fact]
    public void Create_WithValidParameters_CreatesBook()
    {
        // Act
        var result = GeneralBook.Create(_id, Title, Author, _published, Language, _coverPhoto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Id = _id,
            Title,
            Author,
            Published = _published,
            OriginalLanguage = Language,
            CoverPhoto = _coverPhoto
        });
    }

    [Fact]
    public void Reconstitute_CreatesBookWithAllProperties()
    {
        // Arrange
        var rating = new Rating(4.5f);
        var genres = new[] { BookGenre.Fiction, BookGenre.ScienceFiction };
        
        // Act
        var book = GeneralBook.Reconstitute(
            _id, Title, Author, _published, Language, _coverPhoto, rating, 
            genres, Enumerable.Empty<UserBook>(), Enumerable.Empty<Review>());

        // Assert
        book.Should().BeEquivalentTo(new
        {
            Id = _id,
            Title,
            Author,
            Published = _published,
            OriginalLanguage = Language,
            CoverPhoto = _coverPhoto,
            RatingAvg = rating,
            Genres = genres
        });
    }

    [Fact]
    public void UpdateCoverPhoto_ChangesCoverPhoto()
    {
        // Arrange
        var book = GeneralBook.Create(_id, Title, Author, _published, Language, _coverPhoto).Value;
        var newCover = new Photo("new-cover.jpg");
        
        // Act
        book.UpdateCoverPhoto(newCover);
        
        // Assert
        book.CoverPhoto.Should().BeEquivalentTo(newCover);
    }

    [Fact]
    public void UpdateScalarValues_WithNulls_OnlyUpdatesProvidedValues()
    {
        // Arrange
        var book = GeneralBook.Create(_id, Title, Author, _published, Language, _coverPhoto).Value;
        var newTitle = "Updated Title";
        var newPublished = new DateOnly(2021, 1, 1);
        
        // Act
        var result = book.UpdateScalarValues(newTitle, null, newPublished, null);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        book.Should().BeEquivalentTo(new
        {
            Title = newTitle,
            Author, // Should remain unchanged
            Published = newPublished,
            OriginalLanguage = Language // Should remain unchanged
        });
    }

    [Fact]
    public void UpdateRating_ChangesRating()
    {
        // Arrange
        var book = GeneralBook.Create(_id, Title, Author, _published, Language, _coverPhoto).Value;
        var newRating = new Rating(4.8f);
        
        // Act
        book.UpdateRating(newRating);
        
        // Assert
        book.RatingAvg.Should().Be(newRating);
    }
}