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
    public Reputation Reputation { get; private set; } = Reputation.Initial();
    public Photo? ProfilePicture { get; private set; }
    public BioString Bio { get; private set; } = BioString.Initial();

    // Relationships
    // the strategy is - if the relaction is simple eg wishilist is just ids of geenral books that user wants to read then the list is just <Guid> in this case of general book ids
    // if the relation is more complex (eg socialMediaLinks have urls and are represented by sealed records becouse of some custom logic)
    // then the list is of that records in this case <SocialMediaLinks> to represent user owning them and have better mappings and controll over them
    
    
    private readonly WishlistCollection _wishlist;
    public IReadOnlyCollection<Guid> Wishlist => _wishlist.Items;
    private readonly FollowedCollection _followed;
    public IReadOnlyCollection<Guid> Followed => _followed.FollowedUsers;
    private readonly List<Guid> _followers;
    public IReadOnlyCollection<Guid> Followers => _followers.AsReadOnly();
    private readonly BlockedCollection _blocked;
    public IReadOnlyCollection<Guid> Blocked => _blocked.BlockedUsers;


    private readonly SocialMediaCollection _socialMedia;
    public IReadOnlyCollection<SocialMediaLink> SocialMediaLinks => _socialMedia.Links;
    private readonly UserBookCollection _ownedBooks;
    public IReadOnlyCollection<UserBook> OwnedBooks => _ownedBooks.UserBooks;

    // constructor for Create — empty collections
    private User(
        Guid id,
        string email,
        string username,
        string firstName,
        string lastName,
        DateOnly birthDate,
        Location location
    ) : this(
        id, email, username, firstName, lastName, birthDate, 
        location, 
        initialWishlist: Enumerable.Empty<Guid>(),
        initialFollowing: [],
        initialFollowers: [],
        initialBlocked: [],
        initialBooks: [],
        initialSocial: []
    ) { }

    // reconstitution constructor — bulk‑load from persistence
    private User(
        Guid id,
        string email,
        string username,
        string firstName,
        string lastName,
        DateOnly birthDate,
        Location location,
        IEnumerable<Guid> initialWishlist,
        IEnumerable<Guid> initialFollowing,
        IEnumerable<Guid> initialFollowers,
        IEnumerable<Guid> initialBlocked,
        IEnumerable<UserBook> initialBooks,
        IEnumerable<SocialMediaLink> initialSocial
    )
    {
        Id = id;
        Email = email;
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Location = location;

        // now we can assign readonly fields once, here in constructor:
        _wishlist   = new WishlistCollection(initialWishlist);
        _followed   = new FollowedCollection(initialFollowing);
        _followers  = new List<Guid>(initialFollowers);
        _blocked    = new BlockedCollection(initialBlocked);
        _ownedBooks = new UserBookCollection(initialBooks);
        _socialMedia= new SocialMediaCollection(initialSocial); 
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
            errors.Add(DomainErrorFactory.Forbidden("Auth.Underage", "User wass found to be underage"));

        return errors.Count != 0
        ? Result.Fail<User>(errors)
        : Result.Ok( 
            new User(
                Guid.NewGuid(),
                email,
                username,
                firstName,
                lastName,
                birthDate,
                location
            ));
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
        var user = new User(id, email, username, firstName, lastName, birthDate, location,
            initialWishlist:   wishlist,
            initialFollowing:  following,
            initialFollowers:  followers,
            initialBlocked:    blockedUsers,
            initialBooks:      ownedBooks,
            initialSocial:     socialMediaLinks)
        {
            ProfilePicture = profilePicture,
            Reputation     = reputation,
            Bio            = bio,
        };

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