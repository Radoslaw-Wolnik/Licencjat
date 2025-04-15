// Backend.Domain/Entities/User.cs
using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class User : Entity<Guid>
{
    public string Email { get; }
    public string Username { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateOnly BirthDate { get; }
    public Location Location { get; }
    public Reputation Reputation { get; private set; }

    private readonly List<UserBook> _ownedBooks = new(); // insted list of user books
    public IReadOnlyCollection<UserBook> OwnedBooks => _ownedBooks.AsReadOnly();

    private User(
        Guid id,
        string email,
        string username,
        string firstName,
        string lastName,
        DateOnly birthDate,
        Location location,
        Reputation reputation)
    {
        Id = id;
        Email = email;
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Location = location;
        Reputation = reputation;
    }

    public static Result<User> Create(
        string email,
        string username,
        string firstName,
        string lastName,
        DateOnly birthDate,
        Location location)
    {
        var errors = new List<IError>();
        
        if (birthDate > DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-13)))
            errors.Add(UserErrors.Underage);
        
        // we propably should check if location is valid here

        if (errors.Any())
            return Result.Fail<User>(errors);

        return new User(
            Guid.NewGuid(),
            email,
            username,
            firstName,
            lastName,
            birthDate,
            location,
            Reputation.Initial()
        );
    }

    public Result UpdateReputation(float newValue)
    {
        var result = Reputation.Create(newValue);
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        Reputation = result.Value;
        return Result.Ok();
    }

    public Result AddBook(UserBook book)
    {
        if (_ownedBooks.Count >= 100)
            return Result.Fail(UserErrors.MaxBookLimit);

        if (book.OwnerId != Id)
            return Result.Fail(UserErrors.BookOvnershipMismatch);

        _ownedBooks.Add(book);
        return Result.Ok();
    }
}