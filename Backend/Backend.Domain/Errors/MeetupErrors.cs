using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class MeetupErrors
{
    public static DomainError InvalidStatus => new(
        "Meetup.InvalidStatus",
        "Specified meetup status is not valid",
        ErrorType.BadRequest);

}