using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Collections;

public class FollowedCollection
{
    private readonly List<Guid> _followedUsers;
    public IReadOnlyCollection<Guid> FollowedUsers => _followedUsers.AsReadOnly();

    public FollowedCollection(IEnumerable<Guid> ids)
    {
        _followedUsers = ids == null
            ? []
            : [.. ids.Distinct()];
    }

    public Result Add(Guid followId)
    {
        if (_followedUsers.Contains(followId))
            return Result.Fail("Already followed.");
        _followedUsers.Add(followId);
        return Result.Ok();
    }

    public Result Remove(Guid followedId)
    {
        if (!_followedUsers.Remove(followedId))
            return Result.Fail("Not in your followed users.");
        return Result.Ok();
    }
}
