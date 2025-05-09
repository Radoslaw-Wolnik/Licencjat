using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IUserFollowingRepository
{
    Task<IReadOnlyCollection<Guid>> GetFollowingAsync(Guid userId);

    Task<Result> AddAsync(Guid userId, Guid newFollowingId, CancellationToken cancellationToken);
    Task<Result> RemoveAsync(Guid userId, Guid unfollowingId, CancellationToken cancellationToken);
}