using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Collections;

public class BlockedCollection
{
    private readonly List<Guid> _blockedUsers;
    public IReadOnlyCollection<Guid> BlockedUsers => _blockedUsers.AsReadOnly();

    public BlockedCollection(IEnumerable<Guid> ids)
    {
        _blockedUsers = ids == null
            ? []
            : [.. ids.Distinct()];
    }

    public Result Add(Guid blockId)
    {
        if (_blockedUsers.Contains(blockId))
            return Result.Fail("Already blocked.");
        _blockedUsers.Add(blockId);
        // if you block sb that follows you they should unfollow you
        return Result.Ok();
    }

    public Result Remove(Guid blockedId)
    {
        if (!_blockedUsers.Remove(blockedId))
            return Result.Fail("Not in your blocked users.");
        return Result.Ok();
    }
}
