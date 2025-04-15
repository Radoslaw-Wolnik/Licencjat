// Backend.Domain/Entities/Swap.cs
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class Swap : Entity<Guid>
{
    public Guid SubSwapRequestingId { get; }
    public Guid? SubSwapAcceptingId { get; }

    private readonly List<Guid> _meetups = new(); // only two are allowed - one at begging and one at the end
    private readonly List<Guid> _timelineUpdates = new();

    public IReadOnlyCollection<Guid> Meetups => _meetups.AsReadOnly();
    public IReadOnlyCollection<Guid> TimelineUpdates => _timelineUpdates.AsReadOnly();

    private Swap(Guid subSwapRequesitngId) // mby we actually need id in all entites as they are made based on the dbEntities? Although alternatively wehn we first make domain entity and then the dbentity we dont care about ids
    {
        SubSwapRequestingId = subSwapRequesitngId;
    }

    public static Result<Swap> Create(Guid subSwapRequesitngId)
    {
        // if (subSwapAId == subSwapBId)
        //     return Result.Fail(SwapErrors.SameSubSwapError);

        return Result.Ok(new Swap(subSwapRequesitngId));
    }

    public Result AddMeetup(Guid meetupId)
    {
        if (_meetups.Contains(meetupId))
            return Result.Fail(SwapErrors.DuplicateMeetupError);
        
        if(_meetups.Count >= 2)
            return Result.Fail(MeetupErrors.TooMany);
            
        _meetups.Add(meetupId);
        return Result.Ok();
    }
}