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

    private readonly MeetupsCollection _meetups = new(); // only two are allowed - one at begging and one at the end
    private readonly TimelineUpdatesCollection _timelineUpdates = new();

    public IReadOnlyCollection<Meetup> Meetups => _meetups.Meetups;
    public IReadOnlyCollection<TimelineUpdate> TimelineUpdates => _timelineUpdates.TimelineUpdates;

    private Swap(SubSwap subSwapRequesitng, SubSwap subSwapAccepting) // mby we actually need id in all entites as they are made based on the dbEntities? Although alternatively wehn we first make domain entity and then the dbentity we dont care about ids
    {
        SubSwapRequesting = subSwapRequesitng;
        SubSwapAccepting = subSwapAccepting;
    }

    public static Result<Swap> Create(SubSwap subSwapRequesitng, SubSwap subSwapAccepting)
    {
        // if (subSwapAId == subSwapBId)
        //     return Result.Fail(SwapErrors.SameSubSwapError);

        return Result.Ok(new Swap(subSwapRequesitng, subSwapAccepting));
    }

    public static Swap Reconstitute(
        SubSwap subSwapRequesitng,
        SubSwap subSwapAccepting,
        IEnumerable<Meetup>? meetups,
        IEnumerable<TimelineUpdate>? timelineUpdates
    ) {
        var swap = new Swap(subSwapRequesitng, subSwapAccepting);
        foreach(var m in meetups?? []) swap._meetups.Add(m);
        foreach(var tu in timelineUpdates?? []) swap._timelineUpdates.Add(tu);

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