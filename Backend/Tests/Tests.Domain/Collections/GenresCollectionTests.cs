using Backend.Domain.Collections;
using Backend.Domain.Enums;
using FluentAssertions;

namespace Tests.Domain.Collections;

public class GenresCollectionTests
{
    private const BookGenre Genre1 = BookGenre.Fiction;
    private const BookGenre Genre2 = BookGenre.ScienceFiction;

    [Fact]
    public void Add_NewGenre_AddsToCollection()
    {
        // Arrange
        var collection = new GenresCollection(new[] { Genre1 });
        
        // Act
        var result = collection.Add(Genre2);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Genres.Should().BeEquivalentTo([Genre1, Genre2]);
    }

    [Fact]
    public void Add_ExistingGenre_ReturnsFailure()
    {
        // Arrange
        var collection = new GenresCollection(new[] { Genre1 });
        
        // Act
        var result = collection.Add(Genre1);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Already added this genre.");
    }

    [Fact]
    public void Remove_ExistingGenre_RemovesFromCollection()
    {
        // Arrange
        var collection = new GenresCollection(new[] { Genre1, Genre2 });
        
        // Act
        var result = collection.Remove(Genre1);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Genres.Should().BeEquivalentTo([Genre2]);
    }

    [Fact]
    public void Remove_NonExistentGenre_ReturnsFailure()
    {
        // Arrange
        var collection = new GenresCollection(new[] { Genre1 });
        
        // Act
        var result = collection.Remove(Genre2);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("not found");
    }

    [Fact]
    public void Replace_WithNewGenres_UpdatesCollection()
    {
        // Arrange
        var collection = new GenresCollection(new[] { Genre1, BookGenre.Romance });
        var newGenres = new[] { Genre2, BookGenre.Mystery };
        
        // Act
        var result = collection.Replace(newGenres);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Genres.Should().BeEquivalentTo(newGenres);
    }

    [Fact]
    public void Replace_WithOverlappingGenres_KeepsDistinct()
    {
        // Arrange
        var collection = new GenresCollection(new[] { Genre1, Genre2 });
        var newGenres = new[] { Genre2, BookGenre.Biography };
        
        // Act
        var result = collection.Replace(newGenres);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        collection.Genres.Should().BeEquivalentTo([Genre2, BookGenre.Biography]);
    }
}