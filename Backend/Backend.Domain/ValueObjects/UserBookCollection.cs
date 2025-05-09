using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.ValueObjects;

public class UserBookCollection
{
    private readonly List<UserBook> _userBooks = new();
    public IReadOnlyCollection<UserBook> UserBooks => _userBooks.AsReadOnly();

    public Result Add(UserBook book)
    {
        if (_userBooks.Contains(book))
            return Result.Fail("Already added this book.");
        
        _userBooks.Add(book);
        return Result.Ok();
    }

    public Result Remove(Guid bookId)
    {
        var existing = _userBooks.SingleOrDefault(ub => ub.Id == bookId);
        if (existing == null)
            return Result.Fail("User book not found in user library");
        
        _userBooks.Remove(existing);
        return Result.Ok();
    }

    public Result Update(UserBook updatedBook){
        var oldBook = _userBooks.SingleOrDefault(ub => ub.Id == updatedBook.Id);
        if (oldBook == null)
            return Result.Fail("Not in the User Books library");
    
        // replace
        _userBooks.Remove(oldBook);
        _userBooks.Add(updatedBook);
        return Result.Ok();
    }
}
