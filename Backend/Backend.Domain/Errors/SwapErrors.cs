// Domain/Errors/SwapErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class SwapErrors
{
    public static DomainError NotFound => new(
        "Swap.NotFound",
        "Swap request not found",
        ErrorType.NotFound);

    public static DomainError InvalidState => new(
        "Swap.InvalidState",
        "Invalid swap state transition",
        ErrorType.Conflict);

    public static DomainError SwapLimit => new(
        "Swap.Limit",
        "Maximum active swaps reached",
        ErrorType.Validation);
    
    public static DomainError NegativePageNumber => new(
        "Swap.NegativePageNumber",
        "Page number cannot be negative",
        ErrorType.BadRequest);

    public static DomainError SameSubSwapError => new(
        "Swap.SameSubSwap",
        "Cannot swap with the same subswap",
        ErrorType.Conflict);

    public static DomainError DuplicateMeetupError => new(
        "Swap.DuplicateMeetup",
        "Meetup already exists for this swap",
        ErrorType.Conflict);
}