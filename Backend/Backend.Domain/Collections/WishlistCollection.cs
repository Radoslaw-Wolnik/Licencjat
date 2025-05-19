using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.ValueObjects;

public class WishlistCollection
{
    private readonly List<Guid> _items;
    public IReadOnlyCollection<Guid> Items => _items.AsReadOnly();

    public WishlistCollection(IEnumerable<Guid> ids)
    {
        _items = ids == null
            ? new List<Guid>()
            : [.. ids.Distinct()];
    }

    public Result Add(Guid bookId)
    {
        if (_items.Contains(bookId))
            return Result.Fail("Already in wishlist.");
        _items.Add(bookId);
        return Result.Ok();
    }

    public Result Remove(Guid bookId)
    {
        if (!_items.Remove(bookId))
            return Result.Fail("Not in wishlist.");
        return Result.Ok();
    }
}
