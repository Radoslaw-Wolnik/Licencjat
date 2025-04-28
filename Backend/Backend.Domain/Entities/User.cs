// Backend.Domain/Entities/User.cs
using Backend.Domain.Common;
using Backend.Domain.Enums;
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
    public Photo? ProfilePicture { get; private set; }
    public string? Bio { get; private set; }

    // Relationships
    // the strategy is - if the relaction is simple eg wishilist is just ids of geenral books that user wants to read then the list is just <Guid> in this case of general book ids
    // if the relation is more complex (eg socialMediaLinks have urls and are represented by sealed records becouse of some custom logic)
    // then the list is of that records in this case <SocialMediaLinks> to represent user owning them and have better mappings and controll over them
    private readonly List<Guid> _wishlist = new(); // general book id
    private readonly List<Guid> _following = new();
    private readonly List<Guid> _followers = new();
    private readonly List<Guid> _blockedUsers = new();
    // records - more complex relations
    private readonly List<UserBook> _ownedBooks = new();
    private readonly List<SocialMediaLink> _socialMediaLinks = new();
    

    // simple id relations
    public IReadOnlyCollection<Guid> Wishlist => _wishlist.AsReadOnly();
    public IReadOnlyCollection<Guid> Following => _following.AsReadOnly();
    public IReadOnlyCollection<Guid> Followers => _followers.AsReadOnly();
    public IReadOnlyCollection<Guid> BlockedUsers => _blockedUsers.AsReadOnly();

    // records
    public IReadOnlyCollection<UserBook> OwnedBooks => _ownedBooks.AsReadOnly();
    public IReadOnlyCollection<SocialMediaLink> SocialMediaLinks
        => _socialMediaLinks.AsReadOnly();

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
        Photo? profilePicture,
        string? bio,
        IEnumerable<Guid> wishlist, 
        IEnumerable<Guid> following,
        IEnumerable<Guid> followers,
        IEnumerable<Guid> blockedUsers,
        IEnumerable<UserBook> ownedBooks,
        IEnumerable<SocialMediaLink> socialMediaLinks
    ) {
        var user = new User(id, email, username, firstName, lastName, birthDate, location, reputation)
        {
            ProfilePicture = profilePicture,
            Bio = bio
        };
        
        foreach(var w in wishlist) user._wishlist.Add(w);
        foreach(var fing in following) user._following.Add(fing);
        foreach(var flers in followers) user._followers.Add(flers);
        foreach(var bu in blockedUsers) user._blockedUsers.Add(bu);
        foreach(var ob in ownedBooks) user._ownedBooks.Add(ob);
        foreach(var sml in socialMediaLinks) user._socialMediaLinks.Add(sml);

        return user;
    }


    // Core business methods
    public Result UpdateReputation(float newValue)
    {
        var result = Reputation.Create(newValue);
        if (result.IsFailed) return Result.Fail(result.Errors);
        Reputation = result.Value;
        return Result.Ok();
    }

    public Result AddBook(UserBook userBook)
    {
        if (_ownedBooks.Count >= 100) return Result.Fail(UserErrors.MaxBookLimit);
        // if (userBook.OwnerId != Id) return Result.Fail(UserErrors.BookOwnershipMismatch);
        _ownedBooks.Add(userBook);
        return Result.Ok();
    }

    public Result AddSocialMediaLink(SocialMediaLink socilaMediaLink)
    {
        // construct the SocialMediaLink record somewhere else and in user just pass the ready record and check for buisness logic considering the agregation
        // eg the max socialmedialinks ammount
        if (_socialMediaLinks.Count >= 10)
            return Result.Fail(UserErrors.MaxSocialMediaLinks);

        _socialMediaLinks.Add(socilaMediaLink);
        return Result.Ok();
    }

    public Result RemoveSocialMediaLink(Guid linkId)
    {
        var link = _socialMediaLinks.FirstOrDefault(l => l.Id == linkId);
        if (link == null) return Result.Fail("Not found");
        _socialMediaLinks.Remove(link);
        return Result.Ok();
    }

    public Result FollowUser(Guid userId)
    {
        if (_following.Contains(userId)) return Result.Fail(UserErrors.AlreadyFollowing);
        _following.Add(userId);
        return Result.Ok();
    }
}