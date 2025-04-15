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
    public Location Location { get; private set; }
    public Reputation Reputation { get; private set; }
    public string? ProfilePicture { get; private set; }
    public string? Bio { get; private set; }

    // Relationships
    private readonly List<Guid> _wishlist = new();
    private readonly List<Guid> _followedBooks = new();
    private readonly List<Guid> _following = new();
    private readonly List<Guid> _followers = new();
    private readonly List<Guid> _blockedUsers = new();
    private readonly List<Guid> _ownedBooks = new(); // guids to userbook
    private readonly List<SocialMediaLink> _socialMediaLinks = new();

    public IReadOnlyCollection<Guid> Wishlist => _wishlist.AsReadOnly();
    public IReadOnlyCollection<Guid> FollowedBooks => _followedBooks.AsReadOnly();
    public IReadOnlyCollection<Guid> Following => _following.AsReadOnly();
    public IReadOnlyCollection<Guid> Followers => _followers.AsReadOnly();
    public IReadOnlyCollection<Guid> BlockedUsers => _blockedUsers.AsReadOnly();
    public IReadOnlyCollection<Guid> OwnedBooks => _ownedBooks.AsReadOnly();
    public IReadOnlyCollection<SocialMediaLink> SocialMediaLinks => _socialMediaLinks.AsReadOnly();

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
            errors.Add(AuthErrors.Underage);

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



    // Core business methods
    public Result UpdateReputation(float newValue)
    {
        var result = Reputation.Create(newValue);
        if (result.IsFailed) return result.ToResult();
        Reputation = result.Value;
        return Result.Ok();
    }

    public Result AddBook(Guid userBookId)
    {
        if (_ownedBooks.Count >= 100) return Result.Fail(UserErrors.MaxBookLimit);
        // if (userBook.OwnerId != Id) return Result.Fail(UserErrors.BookOwnershipMismatch);
        _ownedBooks.Add(userBookId);
        return Result.Ok();
    }

    public Result AddSocialMediaLink(SocialMediaLink link)
    {
        if (_socialMediaLinks.Count >= 10) return Result.Fail(UserErrors.MaxSocialMediaLinks);
        _socialMediaLinks.Add(link);
        return Result.Ok();
    }

    public Result FollowUser(Guid userId)
    {
        if (_following.Contains(userId)) return Result.Fail(UserErrors.AlreadyFollowing);
        _following.Add(userId);
        return Result.Ok();
    }
}