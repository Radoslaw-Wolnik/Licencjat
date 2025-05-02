namespace Backend.Application.Interfaces.Repositories;

public interface IUserWishlistRepository
{
    Task<IReadOnlyCollection<Guid>> GetByUserIdAsync(Guid userId);

    Task AddAsync(Guid userId, Guid bookId);
    Task RemoveAsync(Guid userId, Guid bookId);
    
}