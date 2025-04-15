// Backend.Domain/Entities/Meetup.cs
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class Meetup : Entity<Guid>
{
    public LocationCoordinates? Location { get; private set; }
    public MeetupStatus Status { get; private set; }
    public Guid SuggestedUserId { get; }
    public Guid SwapId { get; }

    private Meetup(Guid id, LocationCoordinates? location, MeetupStatus status,
        Guid suggestedUserId, Guid swapId)
    {
        Id = id;
        Location = location;
        Status = status;
        SuggestedUserId = suggestedUserId;
        SwapId = swapId;
    }

    public static Result<Meetup> Create(LocationCoordinates? location, MeetupStatus status,
        Guid suggestedUserId, Guid swapId)
    {
        var errors = new List<IError>();
        
        if (suggestedUserId == Guid.Empty) errors.Add(UserErrors.NotFound);
        if (swapId == Guid.Empty) errors.Add(SwapErrors.NotFound);
        if (!Enum.IsDefined(typeof(MeetupStatus), status)) errors.Add(MeetupErrors.InvalidStatus);

        if (errors.Any()) return Result.Fail<Meetup>(errors);

        return new Meetup(
            Guid.NewGuid(),
            location,
            status,
            suggestedUserId,
            swapId
        );
    }

    public Result UpdateLocation(LocationCoordinates newLocation)
    {
        Location = newLocation;
        return Result.Ok();
    }
}