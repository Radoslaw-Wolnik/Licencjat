// Domain/Errors/RelationshipErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class RelationshipErrors
{
    public static DomainError BlockExists => new(
        "Relationship.BlockExists",
        "User is already blocked",
        ErrorType.Conflict);

    public static DomainError BlockNotFound => new(
        "Relationship.BlockNotFound",
        "Block relationship not found",
        ErrorType.NotFound);
}