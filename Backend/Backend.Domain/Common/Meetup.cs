using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record Meetup(Guid Id, Guid SwapId, Guid SuggestedUserId, MeetupStatus Status, LocationCoordinates Location)
{
    public Guid Id { get; private set; } = Id;
    public Guid SwapId { get; private set; } = SwapId;
    public Guid SuggestedUserId { get; } = SuggestedUserId;
    public MeetupStatus Status { get; private set; } = Status;
    public LocationCoordinates Location { get; private set; } = Location;

    public static Result<Meetup> Create(Guid id, Guid swapId, Guid suggestedUserId, MeetupStatus status, LocationCoordinates location)
    {
        var errors = new List<IError>();
        
        if (suggestedUserId == Guid.Empty) errors.Add(UserErrors.NotFound);
        if (!Enum.IsDefined(typeof(MeetupStatus), status)) errors.Add(MeetupErrors.InvalidStatus);

        if (errors.Count != 0) return Result.Fail<Meetup>(errors);

        return new Meetup(
            id,
            swapId,
            suggestedUserId,
            status,
            location
        );
    }
}