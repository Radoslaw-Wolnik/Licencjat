using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces;

namespace Backend.Infrastructure.Repositories.Users;

// Blocked (ID‑only nav)
public interface IUserBlockedRepository
{
    Task<IReadOnlyCollection<Guid>> GetByUserIdAsync(Guid userId);

    Task AddAsync(Guid userId, Guid blockedId);
    Task RemoveAsync(Guid userId, Guid unblockedId);
    
}

public class UserBlockedRepository : IUserBlockedRepository
{
    private readonly ApplicationDbContext _context;

    public UserBlockedRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Guid>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserBlockeds
            .AsNoTracking()
            .Where(x => x.BlockerId == userId)
            .Select(x => x.BlockedId)
            .ToListAsync();
    }

    public async Task AddAsync(Guid userId, Guid blockedId)
    {
        var entity = new UserBlockedEntity {BlockedId = blockedId, BlockerId = userId};
        _context.UserBlockeds.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid userId, Guid unblockedId)
    {
        // var entity = new UserBlockedEntity { BlockerId = userId, BlockedId = unblockedId };
        // _context.UserBlockeds.Attach(entity);
        // _context.UserBlockeds.Remove(entity);
        var existing = await _context.UserBlockeds.FirstOrDefaultAsync(b => b.BlockerId == userId && b.BlockedId == unblockedId);
        if (existing is null)
            throw new KeyNotFoundException($"User with Id = {userId} that blocked Id = {unblockedId} was not found.");

        _context.UserBlockeds.Remove(existing);
        await _context.SaveChangesAsync();  
    }
    

}