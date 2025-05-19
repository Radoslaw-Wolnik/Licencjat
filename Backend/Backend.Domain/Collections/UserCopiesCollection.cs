using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.ValueObjects;

public class UserCopiesCollection
{
    private readonly List<UserBook> _userBooks = new();
    public IReadOnlyCollection<UserBook> UserBooks => _userBooks.AsReadOnly();

    public UserCopiesCollection(IEnumerable<UserBook> userbooks)
    {
        _userBooks = userbooks == null
            ? []
            : [.. userbooks.Distinct()];
    }

    public Result Add(UserBook userBook)
    {
        if (_userBooks.Contains(userBook))
            return Result.Fail("Already added this book.");

        //if (_userBooks.Count(ub => ub.OwnerId == userBook.OwnerId) < 3)
        //    return Result.Fail("One person can only have 3 copies of one book");
        if (_userBooks.Any(ub => ub.OwnerId == userBook.OwnerId))
            return Result.Fail("One person can only have one copy book");
        
        _userBooks.Add(userBook);
        return Result.Ok();
    }

    public Result Remove(Guid userBookId)
    {
        var existing = _userBooks.SingleOrDefault(r => r.Id == userBookId);
        if (existing == null)
            return Result.Fail("not found");
        
        _userBooks.Remove(existing);
        return Result.Ok();
    }

    public Result Update(UserBook updatedUserBook){
        var oldBook = _userBooks.SingleOrDefault(r => r.Id == updatedUserBook.Id);
        if (oldBook == null)
            return Result.Fail("cannot update review that doesnt exsisit");

        // logic
        if (oldBook.OwnerId != updatedUserBook.OwnerId)
            return Result.Fail("cannot update the user owning book");
        
        if (oldBook.GeneralBookId != updatedUserBook.GeneralBookId)
            return Result.Fail("cannot update the book General book reference");
        
        // replace
        _userBooks.Remove(oldBook);
        _userBooks.Add(updatedUserBook);
        return Result.Ok();
    }
}
