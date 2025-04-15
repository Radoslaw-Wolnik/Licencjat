// Backend.Domain/Entities/Meetup.cs
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record Meetup
{
    public Guid SuggestedUserId { get; }
    public MeetupStatus Status { get; private set; }
    public LocationCoordinates? Location { get; private set; }

    private Meetup(Guid suggestedUserId, MeetupStatus? status, LocationCoordinates? location)
    {
        SuggestedUserId = suggestedUserId;
        Status = status ?? MeetupStatus.NoLocation;
        Location = location;
    }

    public static Result<Meetup> Create(Guid suggestedUserId, MeetupStatus? status, LocationCoordinates? location
        )
    {
        var errors = new List<IError>();
        
        if (suggestedUserId == Guid.Empty) errors.Add(UserErrors.NotFound);
        if (status == null) status = MeetupStatus.NoLocation;
        if (!Enum.IsDefined(typeof(MeetupStatus), status)) errors.Add(MeetupErrors.InvalidStatus);

        if (errors.Count != 0) return Result.Fail<Meetup>(errors);

        return new Meetup(
            suggestedUserId,
            status,
            location
        );
    }
}