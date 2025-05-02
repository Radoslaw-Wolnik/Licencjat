namespace Backend.Application.Interfaces.Repositories;

public interface IUserFollowingRepository
{
    Task<IReadOnlyCollection<Guid>> GetFollowingAsync(Guid userId);

    Task AddToFollowingAsync(Guid userId, Guid newFollowingId);
    Task RemoveFromFollowingAsync(Guid userId, Guid unfollowingId);
}