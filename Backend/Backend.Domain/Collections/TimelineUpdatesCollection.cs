using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Collections;

public class TimelineUpdatesCollection
{
    private readonly List<TimelineUpdate> _timelineUpdates;
    public IReadOnlyCollection<TimelineUpdate> TimelineUpdates => _timelineUpdates.AsReadOnly();

    public TimelineUpdatesCollection(IEnumerable<TimelineUpdate> updates)
    {
        _timelineUpdates = updates == null
            ? []
            : [.. updates.Distinct()];
    }

    public Result Add(TimelineUpdate timelineUpdate)
    {
        // can only request once
        if (timelineUpdate.Status == TimelineStatus.Requested && TimelineUpdates.Count != 0)
            return Result.Fail("Can only request once during a timeline of a swap");
        // etc ?
        
        _timelineUpdates.Add(timelineUpdate);
        return Result.Ok();
    }

    public Result Remove(Guid timelineUpdateId)
    {
        var existing = _timelineUpdates.SingleOrDefault(tu => tu.Id == timelineUpdateId);
        if (existing == null)
            return Result.Fail("Timeine update not found in the list");
        
        _timelineUpdates.Remove(existing);
        return Result.Ok();
    }
}
