// Domain/Errors/WishlistErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class WishlistErrors
{
    public static DomainError ItemExists => new(
        "Wishlist.Exists",
        "Item already exists in wishlist",
        ErrorType.Conflict);

    public static DomainError NotFound => new(
        "Wishlist.NotFound",
        "Item not found in user wishlist",
        ErrorType.NotFound);

    public static DomainError LimitReached => new(
        "Wishlist.Limit",
        "Wishlist item limit reached",
        ErrorType.Validation);
}
