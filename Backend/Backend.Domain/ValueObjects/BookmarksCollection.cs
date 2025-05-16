using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.ValueObjects;

public class BookmarksCollection
{
    private readonly List<Bookmark> _bookmarks = new();
    public IReadOnlyCollection<Bookmark> Bookmarks => _bookmarks.AsReadOnly();

    public BookmarksCollection(IEnumerable<Bookmark> bookmarks)
    {
        _bookmarks = bookmarks == null
            ? []
            : [.. bookmarks.Distinct()];
    }

    public Result Add(Bookmark bookmark)
    {
        if (_bookmarks.Contains(bookmark))
            return Result.Fail("Already added this bookmark.");
        
        // chceck for duplicates
        if (_bookmarks.Any(b => b.Description == bookmark.Description && b.Page == bookmark.Page))
            return Result.Fail("Duplicate bookmark");
        
        _bookmarks.Add(bookmark);
        return Result.Ok();
    }

    public Result Remove(Bookmark bookmark)
    {
        if (!_bookmarks.Remove(bookmark))
            return Result.Fail("Not in you book bookmarks.");
        return Result.Ok();
    }
}
