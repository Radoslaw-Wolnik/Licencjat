using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IUserFollowingRepository
{
    Task<Result> AddAsync(Guid userId, Guid newFollowingId, CancellationToken cancellationToken);
    Task<Result> RemoveAsync(Guid userId, Guid unfollowingId, CancellationToken cancellationToken);
}