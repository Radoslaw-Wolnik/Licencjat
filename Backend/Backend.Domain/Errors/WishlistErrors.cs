// Domain/Errors/WishlistErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class WishlistErrors
{
    public static DomainError ItemExists => new(
        "Wishlist.Exists",
        "Item already exists in wishlist",
        ErrorType.Conflict);

    public static DomainError LimitReached => new(
        "Wishlist.Limit",
        "Wishlist item limit reached",
        ErrorType.Validation);
}
