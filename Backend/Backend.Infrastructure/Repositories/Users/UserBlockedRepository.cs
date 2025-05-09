using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;

namespace Backend.Infrastructure.Repositories.Users;

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

    public async Task<Result> AddAsync(Guid userId, Guid blockedId, CancellationToken cancellationToken)
    {
        var entity = new UserBlockedEntity {BlockedId = blockedId, BlockerId = userId};
        _context.UserBlockeds.Add(entity);
        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to add BlockedUser");
        // return result.IsSuccess
        //    ? Result.Ok(entity.Id)
        //    : Result.Fail<Guid>(result.Errors);
    }

    public async Task<Result> RemoveAsync(Guid userId, Guid unblockedId, CancellationToken cancellationToken)
    {
        // var entity = new UserBlockedEntity { BlockerId = userId, BlockedId = unblockedId };
        // _context.UserBlockeds.Attach(entity);
        // _context.UserBlockeds.Remove(entity);
        var existing = await _context.UserBlockeds.FirstOrDefaultAsync(b => b.BlockerId == userId && b.BlockedId == unblockedId);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("UserBlocked", $"{userId} blocking {unblockedId}"));

        _context.UserBlockeds.Remove(existing);

        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to remove BlockedUser");  
    }
    

}