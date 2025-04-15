using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class TimelineErrors
{
    public static DomainError EmptyDescription => new(
        "Timeline.EmptyDescription",
        "Timeline update description cannot be empty",
        ErrorType.BadRequest);

    public static DomainError InvalidStatus => new(
        "Timeline.InvalidStatus",
        "Specified timeline status is not valid",
        ErrorType.BadRequest);
}
