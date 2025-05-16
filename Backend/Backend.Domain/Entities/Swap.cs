using System.ComponentModel;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using Backend.Domain.ValueObjects;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class Swap : Entity<Guid>
{
    public SubSwap SubSwapRequesting { get; }
    public SubSwap SubSwapAccepting { get; }

    private readonly MeetupsCollection _meetups;
    private readonly TimelineUpdatesCollection _timelineUpdates;

    public IReadOnlyCollection<Meetup> Meetups => _meetups.Meetups;
    public IReadOnlyCollection<TimelineUpdate> TimelineUpdates => _timelineUpdates.TimelineUpdates;

    // initial creation
    private Swap(
        Guid requestingUserId, 
        UserBook requestingBook, 
        Guid acceptingUserId
    ) : this (
        Guid.NewGuid(), 
        SubSwap.Initial(requestingUserId, requestingBook), 
        SubSwap.Initial(acceptingUserId, null),
        initialMeetups: Enumerable.Empty<Meetup>(),
        InitialTimelineUpdates: []
    ) { }

    // reconstruction
    private Swap(
        Guid id, 
        SubSwap subSwapRequesting, 
        SubSwap subSwapAccepting, 
        IEnumerable<Meetup> initialMeetups, 
        IEnumerable<TimelineUpdate> InitialTimelineUpdates
    ) {
        Id = id;
        SubSwapAccepting = subSwapAccepting;
        SubSwapRequesting = subSwapRequesting;
        _meetups = new MeetupsCollection(initialMeetups);
        _timelineUpdates = new TimelineUpdatesCollection(InitialTimelineUpdates);
    }

    public static Result<Swap> Create(Guid requestingUserId, UserBook requestingBook, Guid acceptingUserId)
    {
        if (requestingUserId == Guid.Empty)
            return Result.Fail(DomainErrorFactory.NotFound("swap.userRequesting", requestingUserId));
        if (acceptingUserId == Guid.Empty)
            return Result.Fail(DomainErrorFactory.NotFound("swap.userAccepting", acceptingUserId));

        return Result.Ok(new Swap(requestingUserId, requestingBook, acceptingUserId));
    }

    public static Swap Reconstitute(
        Guid id,
        SubSwap subSwapRequesitng,
        SubSwap subSwapAccepting,
        IEnumerable<Meetup> meetups,
        IEnumerable<TimelineUpdate> timelineUpdates
    ) {
        var swap = new Swap(id, subSwapRequesitng, subSwapAccepting, meetups, timelineUpdates);

        return swap;
    }

    public Result AddMeetup(Meetup meetup)
        => _meetups.Add(meetup);
    
    public Result RemoveMeetup(Guid meetupId)
        => _meetups.Remove(meetupId);
    
    public Result UpdateMeetup(Meetup updatedMeetup)
        => _meetups.Update(updatedMeetup);
    
    public Result AddTimelineUpdate(TimelineUpdate timelineUpdate)
        => _timelineUpdates.Add(timelineUpdate);
    
    public Result RemoveTimelineUpdate(Guid timelineUpdateId)
        => _timelineUpdates.Remove(timelineUpdateId);
}