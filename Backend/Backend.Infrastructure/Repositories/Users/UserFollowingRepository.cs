using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;

namespace Backend.Infrastructure.Repositories.Users;

public class UserFollowingRepository : IUserFollowingRepository
{
    private readonly ApplicationDbContext _context;

    public UserFollowingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> AddAsync(Guid userId, Guid newFollowingId, CancellationToken cancellationToken)
    {
        // add
        // var entity = new UserFollowingEntity {FollowerId = userId, FollowedId = newFollowingId};
        // _context.UserFollowings.Add(entity);
        var link = new UserFollowingEntity {
            Id         = Guid.NewGuid(),
            FollowerId = userId,
            FollowedId = newFollowingId
        };
        _context.UserFollowings.Add(link);

        return await _context.SaveChangesWithResultAsync(cancellationToken, "failed to add FollowedUser");
    }


    public async Task<Result> RemoveAsync(Guid userId, Guid unfollowingId, CancellationToken cancellationToken)
    {
        // find the single join‐row
        var link = await _context.UserFollowings
            .FirstOrDefaultAsync(f =>
                f.FollowerId == userId
             && f.FollowedId == unfollowingId);

        if (link is null)
            return Result.Fail(DomainErrorFactory.NotFound("Following", $"{userId} unfollowing {unfollowingId}"));

        _context.UserFollowings.Remove(link);
        return await _context.SaveChangesWithResultAsync(cancellationToken, "Filed to remove Followeduser");
    }

}