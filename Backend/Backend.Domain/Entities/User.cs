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
    private List<Guid> _wishlist = new();
    private List<Guid> _followedBooks = new();
    private List<Guid> _following = new();
    private List<Guid> _followers = new();
    private List<Guid> _blockedUsers = new();
    private List<Guid> _ownedBooks = new(); // guids to userbook
    private List<Guid> _socialMediaLinks = new();

    public IReadOnlyCollection<Guid> Wishlist => _wishlist.AsReadOnly();
    public IReadOnlyCollection<Guid> FollowedBooks => _followedBooks.AsReadOnly();
    public IReadOnlyCollection<Guid> Following => _following.AsReadOnly();
    public IReadOnlyCollection<Guid> Followers => _followers.AsReadOnly();
    public IReadOnlyCollection<Guid> BlockedUsers => _blockedUsers.AsReadOnly();
    public IReadOnlyCollection<Guid> OwnedBooks => _ownedBooks.AsReadOnly();
    public IReadOnlyCollection<Guid> SocialMediaLinks => _socialMediaLinks.AsReadOnly();

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

        return errors.Count != 0
        ? Result.Fail<User>(errors)
        : new User(
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

    // Reconstitution method (for loading from DB)
    public static User Reconstitute(
        Guid id,
        string email,
        string username,
        string firstName,
        string lastName,
        DateOnly birthDate,
        Location location,
        Reputation reputation,
        string? profilePicture,
        string? bio,
        IEnumerable<Guid> wishlist,
        IEnumerable<Guid> followedBooks,
        IEnumerable<Guid> following,
        IEnumerable<Guid> followers,
        IEnumerable<Guid> blockedUsers,
        IEnumerable<Guid> ownedBooks,
        IEnumerable<Guid> socialMediaLinks
    )
    {
        return new User(id, email, username, firstName, lastName, birthDate, location, reputation)
        {
            ProfilePicture = profilePicture,
            Bio = bio,
            _wishlist = wishlist.ToList(),
            _followedBooks = followedBooks.ToList(),
            _following = following.ToList(),
            _followers = followers.ToList(),
            _blockedUsers = blockedUsers.ToList(),
            _ownedBooks = ownedBooks.ToList(),
            _socialMediaLinks = socialMediaLinks.ToList()
        };
        /*
        user.HydrateCollections(wishlist,
            followedBooks,
            following,
            followers,
            blockedUsers,
            ownedBooks,
            socialMediaLinks);
        */
    }

     // Collection hydration method
    public void HydrateCollections(
        IEnumerable<Guid> wishlist,
        IEnumerable<Guid> followedBooks,
        IEnumerable<Guid> following,
        IEnumerable<Guid> followers,
        IEnumerable<Guid> blockedUsers,
        IEnumerable<Guid> ownedBooks,
        IEnumerable<Guid> socialMediaLinks
    )
    {
        _wishlist = wishlist.ToList();
        _followedBooks = followedBooks.ToList();
        _following = following.ToList();
        _followers = followers.ToList();
        _blockedUsers = blockedUsers.ToList();
        _ownedBooks = ownedBooks.ToList();
        _socialMediaLinks = socialMediaLinks.ToList();
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

    public Result AddSocialMediaLink(Guid link)
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