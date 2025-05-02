namespace Backend.Application.Interfaces.Repositories;

public interface IUserBlockedRepository
{
    Task<IReadOnlyCollection<Guid>> GetByUserIdAsync(Guid userId);

    Task AddAsync(Guid userId, Guid blockedId);
    Task RemoveAsync(Guid userId, Guid unblockedId);
    
}