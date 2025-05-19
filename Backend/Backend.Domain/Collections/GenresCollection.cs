using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Collections;

public class GenresCollection
{
    private readonly List<BookGenre> _genres;
    public IReadOnlyCollection<BookGenre> Genres => _genres.AsReadOnly();

    public GenresCollection(IEnumerable<BookGenre> genres)
    {
        _genres = genres == null
            ? []
            : [.. genres.Distinct()];
    }

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

    public Result Replace(IEnumerable<BookGenre> newGenres)
    {
        // _genres.Clear();
        // if (newGenres != null)
        // {
        //    foreach (var g in newGenres.Distinct())
        //        _genres.Add(g);
        // }

        newGenres = newGenres?.Distinct() ?? Enumerable.Empty<BookGenre>();

        // Remove ones that are no longer present
        foreach (var old in _genres.Except(newGenres).ToList())
            _genres.Remove(old);

        // Add the new ones that weren’t already there
        foreach (var nxt in newGenres.Except(_genres))
            _genres.Add(nxt);
        return Result.Ok();
    }
    
}
