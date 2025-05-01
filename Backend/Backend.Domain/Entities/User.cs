using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using Backend.Domain.ValueObjects;
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
    public BioString Bio { get; private set; }

    // Relationships
    // the strategy is - if the relaction is simple eg wishilist is just ids of geenral books that user wants to read then the list is just <Guid> in this case of general book ids
    // if the relation is more complex (eg socialMediaLinks have urls and are represented by sealed records becouse of some custom logic)
    // then the list is of that records in this case <SocialMediaLinks> to represent user owning them and have better mappings and controll over them
    
    
    private readonly WishlistCollection _wishlist = new();
    public IReadOnlyCollection<Guid> Wishlist => _wishlist.Items;
    private readonly FollowedCollection _followed = new();
    public IReadOnlyCollection<Guid> Followed => _followed.FollowedUsers;
    private readonly List<Guid> _followers = new();
    public IReadOnlyCollection<Guid> Followers => _followers.AsReadOnly();
    private readonly BlockedCollection _blocked = new();
    public IReadOnlyCollection<Guid> Blocked => _blocked.BlockedUsers;


    private readonly SocialMediaCollection _socialMedia = new();
    public IReadOnlyCollection<SocialMediaLink> SocialMediaLinks => _socialMedia.Links;
    private readonly USerBookCollection _ownedBooks = new();
    public IReadOnlyCollection<UserBook> OwnedBooks => _ownedBooks.UserBooks;


    private User(
        Guid id,
        string email,
        string username,
        string firstName,
        string lastName,
        DateOnly birthDate,
        Location location,
        Reputation reputation,
        BioString bio)
    {
        Id = id;
        Email = email;
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Location = location;
        Reputation = reputation;
        Bio = bio;
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
            Reputation.Initial(),
            BioString.Initial()
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
        BioString bio,
        IEnumerable<Guid> wishlist, 
        IEnumerable<Guid> following,
        IEnumerable<Guid> followers,
        IEnumerable<Guid> blockedUsers,
        IEnumerable<UserBook> ownedBooks,
        IEnumerable<SocialMediaLink> socialMediaLinks
    ) {
        var user = new User(id, email, username, firstName, lastName, birthDate, location, reputation, bio)
        {
            ProfilePicture = profilePicture,
        };
        
        foreach(var w in wishlist) user._wishlist.Add(w);
        foreach(var fing in following) user._followed.Add(fing);
        foreach(var flers in followers) user._followers.Add(flers);
        foreach(var bu in blockedUsers) user._blocked.Add(bu);

        foreach(var ob in ownedBooks) user._ownedBooks.Add(ob);
        foreach (var sml in socialMediaLinks) user._socialMedia.Add(sml);

        return user;
    }

    // Core business methods

    public Result AddToWishlist(Guid bookId) 
        => _wishlist.Add(bookId);

    public Result RemoveFromWishlist(Guid bookId)
        => _wishlist.Remove(bookId);
    
    public Result AddToFollowed(Guid userId) 
        => _followed.Add(userId);

    public Result RemoveFromFollowed(Guid userId)
        => _followed.Remove(userId);
    
    public Result AddToBlocked(Guid userId) 
        => _blocked.Add(userId);

    public Result RemoveFromBlocked(Guid userId)
        => _blocked.Remove(userId);
    


    public Result AddUserBook(UserBook book)
        => _ownedBooks.Add(book);

    public Result RemoveUserBook(Guid bookId)
        => _ownedBooks.Remove(bookId);

    public Result UpdateUserBook(UserBook updatedBook)
        => _ownedBooks.Update(updatedBook);

    public Result AddSocialMediaLink(SocialMediaLink link)
        => _socialMedia.Add(link);

    public Result RemoveSocialMediaLink(Guid linkId)
        => _socialMedia.Remove(linkId);

    public Result UpdateSocialMediaLink(SocialMediaLink updatedLink)
        => _socialMedia.Update(updatedLink);
    
    
    public void UpdateReputation(Reputation updtedReputation)
        => Reputation = updtedReputation;
    
    public void UpdateProfilePicture(Photo updatedPhoto)
        => ProfilePicture = updatedPhoto;    

    public void RemoveProfilePicture()
        => ProfilePicture = null;
    
    public void UpdateBio(BioString newBio)
        => Bio = newBio;

    public void updateLocation(Location newLocation)
        => Location = newLocation;
}