using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IUserBlockedRepository
{
    Task<IReadOnlyCollection<Guid>> GetByUserIdAsync(Guid userId);

    Task<Result> AddAsync(Guid userId, Guid blockedId, CancellationToken cancellationToken);
    Task<Result> RemoveAsync(Guid userId, Guid unblockedId, CancellationToken cancellationToken);
    
}