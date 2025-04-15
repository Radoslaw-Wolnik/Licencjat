using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class MeetupErrors
{
    public static DomainError InvalidStatus => new(
        "Meetup.InvalidStatus",
        "Specified meetup status is not valid",
        ErrorType.BadRequest);

    public static DomainError TooMany => new(
        "Meetup.TooMany",
        "Only two meetings are allowed per swap",
        ErrorType.BadRequest);
    
    public static DomainError AlreadyConfirmed => new(
        "Meetup.AlreadyConfirmed",
        "The meetup was already confirmed or done",
        ErrorType.BadRequest);

}