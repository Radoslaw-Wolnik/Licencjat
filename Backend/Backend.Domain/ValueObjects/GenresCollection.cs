using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.ValueObjects;

public class GenresCollection
{
    private readonly List<BookGenre> _genres = new();
    public IReadOnlyCollection<BookGenre> Genres => _genres.AsReadOnly();

    public Result Add(BookGenre genre)
    {
        if (_genres.Contains(genre))
            return Result.Fail("Already added this genre.");
        
        _genres.Add(genre);
        return Result.Ok();
    }

    public Result Remove(BookGenre genre)
    {
        if (!_genres.Contains(genre))
            return Result.Fail("not found");
        
        _genres.Remove(genre);
        return Result.Ok();
    }
    
}
