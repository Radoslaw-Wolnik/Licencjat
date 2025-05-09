using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IUserWishlistRepository
{
    Task<IReadOnlyCollection<Guid>> GetByUserIdAsync(Guid userId);

    Task<Result> AddAsync(Guid userId, Guid bookId, CancellationToken cancellationToken);
    Task<Result> RemoveAsync(Guid userId, Guid bookId, CancellationToken cancellationToken);
    
}