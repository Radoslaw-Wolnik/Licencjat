# Domain Layer - The Core of BookSwap  
*Contains all enterprise-wide business rules and domain models*  

<!-- 
![Domain Layer Position](https://miro.medium.com/v2/resize:fit:720/format:webp/1*66isW6uUtN_dd6Y6pqKUHA.png)  
*The innermost layer with no external dependencies*
-->

## 🧠 Responsibility
- Defines core business entities and rules
- Contains pure business logic (no infrastructure concerns)
- Models domain concepts through:
  - Aggregates & Entities
  - Value Objects
  - Domain Events (currently not implemented)
  - Enums & Custom Types
  - Domain Exceptions
  - Collection classes for relationship management

**Dependency Rule**:  
All other layers depend on Domain, but Domain depends on **nothing** outside itself.

---

## 🔑 Key Components

### 1. Aggregates & Entities
Main domain models representing core business concepts:

#### `User` Aggregate Root
```csharp
public sealed class User : Entity<Guid>
{
    public string Email { get; }
    public string Username { get; }
    public Location Location { get; private set; }
    public Reputation Reputation { get; private set; }
    // Collections for relationships
    private readonly WishlistCollection _wishlist;
    private readonly UserBookCollection _ownedBooks;
    private readonly SocialMediaCollection _socialMedia;
    
    // Business methods
    public Result AddToWishlist(Guid bookId) 
    public void UpdateLocation(Location newLocation)
    // ... other domain behaviors
}
```

#### `GeneralBook` Entity
```csharp
public sealed class GeneralBook : Entity<Guid>
{
    public string Title { get; private set; }
    public string Author { get; private set; }
    public Rating RatingAvg { get; private set; }
    // Collections
    private readonly GenresCollection _genres;
    private readonly UserCopiesCollection _userCopies;
    
    public Result UpdateScalarValues(string? title, string? author)
    public Result AddUserCopy(UserBook userBook)
    // ... other behaviors
}
```

#### `UserBook` Entity
```csharp
public sealed class UserBook : Entity<Guid>
{
    public Guid OwnerId { get; }
    public Guid GeneralBookId { get; }
    public BookStatus Status { get; private set; }
    // Value Objects
    public Photo CoverPhoto { get; private set; }
    private readonly BookmarksCollection _bookmarks;
    
    public void UpdateStatus(BookStatus newStatus)
    public Result AddBookmark(Bookmark bookmark)
    // ... other behaviors
}
```

#### `Swap` Aggregate Root
```csharp
public sealed class Swap : Entity<Guid>
{
    public SwapStatus Status { get; private set; }
    public SubSwap SubSwapRequesting { get; }
    public SubSwap SubSwapAccepting { get; }
    private readonly MeetupsCollection _meetups;
    
    public Result InitialBookReading(Guid userId, UserBook userBook)
    public Result UpdatePageReading(Guid userId, int page)
    // ... swap lifecycle management
}
```

---

### 2. Value Objects
Immutable objects with validation logic:

```csharp
public sealed record Email
{
    public string Value { get; }
    
    private Email(string value) => Value = value;

    public static Result<Email> Create(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") 
            ? new Email(email)
            : Result.Failure<Email>(DomainErrors.User.InvalidEmail);
    }
}

// Other value objects:
// - Reputation
// - Location
// - Photo
// - BioString
// - SocialMediaLink
// - LocationCoordinates
// - Rating
// - CountryCode
```

**Characteristics**:  
✅ Encapsulate validation logic  
✅ Immutable by design  
✅ No identity (equality by value)  
✅ Domain-specific behaviors  

---

### 3. Collection Classes
Specialized classes for managing relationships and invariants:

```csharp
// Example: BookmarksCollection
public class BookmarksCollection
{
    private readonly List<Bookmark> _bookmarks = new();
    
    public Result Add(Bookmark bookmark)
    {
        // Check for duplicates
        // Enforce business rules
    }
    
    public Result Remove(Guid bookmarkId)
    {
        // Validation and removal logic
    }
}

// Other collection types:
// - SocialMediaCollection
// - UserBookCollection
// - WishlistCollection
// - MeetupsCollection
// - TimelineUpdatesCollection
```

---

### 4. Enums & Constants
Centralized domain-specific types:

```csharp
public enum BookStatus
{
    Available,
    Borrowed,
    Unavailable,
    ReadingInProgress
}

public enum SwapStatus
{
    Requested,
    Accepted,
    MeetingScheduled,
    Completed,
    Cancelled
}

// Thumbnail specification
public sealed class ThumbnailType
{
    public static readonly ThumbnailType ProfilePicture = new("ProfilePicture", 40, 40);
    public static readonly ThumbnailType Cover = new("Cover", 150, 200);
}
```

---

### 5. Error Handling
Structured error system with factory pattern:

```csharp
// DomainError record
public record DomainError(
    string Code, 
    string Message, 
    ErrorType Type
) : IError;

// Error factory
public static class DomainErrorFactory
{
    public static DomainError NotFound(string entityName, object key)
    {
        return new DomainError(
            $"{entityName}.NotFound",
            $"{entityName} with id '{key}' not found",
            ErrorType.NotFound
        );
    }
    
    // Other error types: Validation, Conflict, etc.
}
```

---

### 6. 🏭 Factory Pattern
Complex object creation with validation:

```csharp
// TimelineUpdate factory
public static class TimelineUpdateFactory
{
    public static Result<TimelineUpdate> CreateRequested(
        Guid userId, 
        Guid swapId)
    {
        return TimelineUpdate.Create(
            Guid.NewGuid(),
            userId,
            swapId,
            TimelineStatus.Requested,
            "Swap requested by user."
        );
    }
    
    // Other factory methods for different statuses
}
```

---

## Design Principles

1. **Rich Domain Models**  
   - Entities contain both state and behavior
   - Encapsulation of state changes through methods
   ```csharp
   // Instead of direct property access:
   userBook.Status = newStatus;
   
   // We use controlled methods:
   userBook.UpdateStatus(newStatus);
   ```

2. **Relationship Management**  
   - Specialized collection classes enforce business rules
   - Prevent invalid state through encapsulated collections
   ```csharp
   // Adding to wishlist with validation
   user.AddToWishlist(bookId);
   ```

3. **Persistence Ignorance**  
   - No database-specific attributes
   - Pure C# implementations
   ```csharp
   // Entity base class
   public abstract class Entity<TId>
   {
       public TId Id { get; protected set; } = default!;
   }
   ```

4. **Ubiquitous Language**  
   - Domain terms match business concepts
   - Clear correspondence with user mental model
   ```csharp
   SwapStatus.Requested
   BookStatus.ReadingInProgress
   SubSwap.UpdatePageAt()
   ```

---

## ⚙️ Usage Example
```csharp
// Creating a book with validation
var userBookResult = UserBook.Create(
    id: null,
    ownerId: userId,
    generalBookId: bookId,
    status: BookStatus.Available,
    state: BookState.New,
    language: LanguageCode.EN,
    pageCount: 300,
    coverPhoto: photo
);

if (userBookResult.IsFailed)
    return userBookResult.ToResult();

// Updating domain state
userBookResult.Value.UpdateStatus(BookStatus.Borrowed);

// Handling errors
var error = DomainErrorFactory.NotFound("User", userId);
return Result.Fail(error);
```

---

## 🔍 What's Next
- [Application Layer](../Backend.Application/README.md)
- [Infrastructure Layer](../Backend.Infrastructure/README.md)
- [API Layer](../Backend.API/README.md)

> "An architect should know the domain better than anyone else"  
> *- Robert C. Martin (Uncle Bob)*