using AutoMapper;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories.Users;

// Following (ID‑only nav)
public interface IUserFollowingRepository
{
    Task<Result<IReadOnlyCollection<Guid>>> GetFollowingAsync(Guid userId);

    Task<Result> AddToFollowingAsync(Guid userId, Guid newFollowingId);
    Task<Result> RemoveFromFollowingAsync(Guid userId, Guid unfollowingId);
    
    Task<Result<bool>> FollowingContainsAsync(Guid userId, Guid candidateId);
    
}

public class UserFollowingRepository : IUserFollowingRepository
{
    private readonly ApplicationDbContext _context;

    public UserFollowingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyCollection<Guid>>> GetFollowingAsync(Guid userId)
    {
        var list = await _context.UserFollowings
            .Where(uf => uf.FollowerId == userId)
            .Select(uf => uf.FollowedId)
            .ToListAsync();

        return Result.Ok((IReadOnlyCollection<Guid>)list);
    }

    public async Task<Result> AddToFollowingAsync(Guid userId, Guid newFollowingId)
    {
        if (userId == newFollowingId)
            return Result.Fail(FollowingErrors.CannotFollowYourself);

        // check if the target user exists
        var target = await _context.Users
            .AnyAsync(u => u.Id == newFollowingId);
        if (!target)
            return Result.Fail(UserErrors.NotFound);

        // prevent duplicate follow
        var already = await _context.UserFollowings
            .AnyAsync(f => f.FollowerId == userId
                        && f.FollowedId == newFollowingId);
        if (already)
            return Result.Fail(FollowingErrors.AlreadyFollowing);

        // add
        _context.UserFollowings.Add(new UserFollowingEntity {
            Id         = Guid.NewGuid(),
            FollowerId = userId,
            FollowedId = newFollowingId
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


    public async Task<Result> RemoveFromFollowingAsync(Guid userId, Guid unfollowingId)
    {
        // find the single join‐row
        var link = await _context.UserFollowings
            .FirstOrDefaultAsync(f =>
                f.FollowerId == userId
             && f.FollowedId == unfollowingId);

        if (link is null)
            return Result.Fail(FollowingErrors.NotFound);

        _context.UserFollowings.Remove(link);
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


    public async Task<Result<bool>> FollowingContainsAsync(Guid userId, Guid candidateId)
    {
        var exists = await _context.UserFollowings
            .AnyAsync(uf =>
                uf.FollowerId == userId 
                && uf.FollowedId == candidateId);
        return Result.Ok(exists);
    }

}