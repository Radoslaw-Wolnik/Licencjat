using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Collections;

public class MeetupsCollection
{
    private readonly List<Meetup> _meetups; // idk if allow only two or not - like one at begging one at the end
    public IReadOnlyCollection<Meetup> Meetups => _meetups.AsReadOnly();

    public MeetupsCollection(IEnumerable<Meetup> meetups)
    {
        _meetups = meetups == null
            ? []
            : [.. meetups.Distinct()];
    }

    public Result Add(Meetup meetup)
    {
        if (_meetups.Count >= 10)
            return Result.Fail("Max meetups count reached");
        
        _meetups.Add(meetup);
        return Result.Ok();
    }

    public Result Remove(Guid meetupId)
    {
        var existing = _meetups.SingleOrDefault(m => m.Id == meetupId);
        if (existing == null)
            return Result.Fail("not found");
        
        _meetups.Remove(existing);
        return Result.Ok();
    }

    public Result Update(Meetup updatedMeetup){
        var oldMeetup = _meetups.SingleOrDefault(m => m.Id == updatedMeetup.Id);
        if (oldMeetup == null)
            return Result.Fail("Not in the Swap Meetups");

        // logic
        
        // replace
        _meetups.Remove(oldMeetup);
        _meetups.Add(updatedMeetup);
        return Result.Ok();
    }
}
