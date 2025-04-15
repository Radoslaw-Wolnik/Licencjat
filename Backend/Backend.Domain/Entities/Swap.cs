// Backend.Domain/Entities/Swap.cs
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class Swap : Entity<Guid>
{
    public Guid SubSwapAId { get; }
    public Guid SubSwapBId { get; }
    private readonly List<Guid> _meetups = new();
    private readonly List<Guid> _timelineUpdates = new();

    public IReadOnlyCollection<Guid> Meetups => _meetups.AsReadOnly();
    public IReadOnlyCollection<Guid> TimelineUpdates => _timelineUpdates.AsReadOnly();

    private Swap(Guid id, Guid subSwapAId, Guid subSwapBId)
    {
        Id = id;
        SubSwapAId = subSwapAId;
        SubSwapBId = subSwapBId;
    }

    public static Result<Swap> Create(Guid subSwapAId, Guid subSwapBId)
    {
        if (subSwapAId == subSwapBId)
            return Result.Fail(SwapErrors.SameSubSwapError);

        return new Swap(Guid.NewGuid(), subSwapAId, subSwapBId);
    }

    public Result AddMeetup(Guid meetupId)
    {
        if (_meetups.Contains(meetupId))
            return Result.Fail(SwapErrors.DuplicateMeetupError);
            
        _meetups.Add(meetupId);
        return Result.Ok();
    }
}