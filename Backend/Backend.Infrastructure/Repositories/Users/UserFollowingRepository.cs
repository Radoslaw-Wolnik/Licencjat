using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;

namespace Backend.Infrastructure.Repositories.Users;

public class UserFollowingRepository : IUserFollowingRepository
{
    private readonly ApplicationDbContext _context;

    public UserFollowingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Guid>> GetFollowingAsync(Guid userId)
    {
        return await _context.UserFollowings
            .AsNoTracking()
            .Where(uf => uf.FollowerId == userId)
            .Select(uf => uf.FollowedId)
            .ToListAsync();
    }

    public async Task AddToFollowingAsync(Guid userId, Guid newFollowingId)
    {
        // add
        // var entity = new UserFollowingEntity {FollowerId = userId, FollowedId = newFollowingId};
        // _context.UserFollowings.Add(entity);
        _context.UserFollowings.Add(new UserFollowingEntity {
            Id         = Guid.NewGuid(),
            FollowerId = userId,
            FollowedId = newFollowingId
        });
        await _context.SaveChangesAsync();
    }


    public async Task RemoveFromFollowingAsync(Guid userId, Guid unfollowingId)
    {
        // find the single join‐row
        var link = await _context.UserFollowings
            .FirstOrDefaultAsync(f =>
                f.FollowerId == userId
             && f.FollowedId == unfollowingId);

        if (link is null)
            throw new KeyNotFoundException($"User {userId} doesnt follow {unfollowingId}");

        _context.UserFollowings.Remove(link);
        await _context.SaveChangesAsync();
    }

}