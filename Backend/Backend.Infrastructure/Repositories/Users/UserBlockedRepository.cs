using AutoMapper;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories.Users;

// Blocked (ID‑only nav)
public interface IUserBlockedRepository
{
    Task<Result<IReadOnlyCollection<Guid>>> GetBlockedAsync(Guid userId);

    Task<Result> AddToBlockedAsync(Guid userId, Guid newBlockedId);
    Task<Result> RemoveFromBlockedAsync(Guid userId, Guid unblockedId);
    
    Task<Result<bool>> BlockedContainsAsync(Guid userId, Guid candidateId);
    
}

public class UserBlockedRepository : IUserBlockedRepository
{
    private readonly ApplicationDbContext _context;

    public UserBlockedRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyCollection<Guid>>> GetBlockedAsync(Guid userId)
    {
        var list = await _context.UserBlockeds
            .Where(ub => ub.BlockerId == userId)
            .Select(ub => ub.BlockedId)
            .ToListAsync();

        return Result.Ok((IReadOnlyCollection<Guid>)list);
    }

    public async Task<Result> AddToBlockedAsync(Guid userId, Guid newBlockedId)
    {
        if (userId == newBlockedId)
            return Result.Fail(BlockedErrors.CannotBlockYourself);

        // check if the target user exists
        var target = await _context.Users
            .AnyAsync(u => u.Id == newBlockedId);
        if (!target)
            return Result.Fail(UserErrors.NotFound);

        // prevent duplicate blocked
        var already = await _context.UserBlockeds
            .AnyAsync(f => f.BlockerId == userId
                        && f.BlockedId == newBlockedId);
        if (already)
            return Result.Fail(BlockedErrors.AlreadyBlocked);

        // add
        _context.UserBlockeds.Add(new UserBlockedEntity {
            Id        = Guid.NewGuid(),
            BlockerId = userId,
            BlockedId = newBlockedId
        });

        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new DomainError(
                "DatabaseError", ex.Message, ErrorType.StorageError));
        }
    }


    public async Task<Result> RemoveFromBlockedAsync(Guid userId, Guid unblockedId)
    {
        // find the single join‐row
        var link = await _context.UserBlockeds
            .FirstOrDefaultAsync(f =>
                f.BlockerId == userId
             && f.BlockedId == unblockedId);

        if (link is null)
            return Result.Fail(FollowingErrors.NotFound);

        _context.UserBlockeds.Remove(link);
        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new DomainError(
                "DatabaseError", ex.Message, ErrorType.StorageError));
        }
    }


    public async Task<Result<bool>> BlockedContainsAsync(Guid userId, Guid candidateId)
    {
        var exists = await _context.UserBlockeds
            .AnyAsync(uf =>
                uf.BlockerId == userId 
                && uf.BlockedId == candidateId);
        return Result.Ok(exists);
    }

}