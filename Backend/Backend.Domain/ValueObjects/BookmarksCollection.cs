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

    public Result Remove(Guid bookmarkId)
    {
        var existing = _bookmarks.SingleOrDefault(b => b.Id == bookmarkId);
        if (existing == null)
            return Result.Fail("Bookmark not found in the bookmarks of this book");
        
        _bookmarks.Remove(existing);
        return Result.Ok();
    }

    public Result Update(Bookmark updatedBookmark)
    {
        var oldBookmark = _bookmarks.SingleOrDefault(b => b.Id == updatedBookmark.Id);
        if (oldBookmark == null)
            return Result.Fail("Bookmark youre trying to update doesnt exsist");
        
        // logic
        if (oldBookmark.UserBookId != updatedBookmark.UserBookId)
            return Result.Fail("cannot update the userBook of the bookmark");
        
        // replace
        _bookmarks.Remove(oldBookmark);
        _bookmarks.Add(updatedBookmark);
        
        return Result.Ok();
    }
}
