using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Common;
using Backend.Application.DTOs;
using AutoMapper.Extensions.ExpressionMapping;

namespace Backend.Infrastructure.Repositories.Users;

public class WriteUserRepository : IWriteUserRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public WriteUserRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> AddAsync(User user, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<UserEntity>(user);
        _db.Users.Add(entity);
        var result = await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to save user");
        
        return result.IsSuccess
            ? Result.Ok(entity.Id)
            : Result.Fail<Guid>(result.Errors);
        // user.SetId(entity.Id); 
    }

    public async Task<Result> DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        var existing = await _db.Users.FindAsync(userId);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("User", userId));

        _db.Users.Remove(existing);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to delete User");
    }

    public async Task<Result> UpdateAsync(
        User user, 
        CancellationToken cancellationToken,
        params Expression<Func<UserProjection, object>>[] includes)
    {
        var query = _db.Users.AsQueryable();

        foreach (var include in includes)
        {
            var entityInclude = _mapper.MapExpression<Expression<Func<UserEntity, object>>>(include);
            query = query.Include(entityInclude);
        }


        var existing = await query.FirstOrDefaultAsync(u => u.Id == user.Id);
        if (existing == null) return Result.Fail(DomainErrorFactory.NotFound("User", user.Id));

        // Map only scalar properties (EF Core will track relationship changes)
        _mapper.Map(user, existing);

        // Explicitly handle collections
        await HandleCollectionUpdates(user, existing); // can we await not sync function? 
        
        return await _db.SaveChangesWithResultAsync(cancellationToken);
    }

    private Task HandleCollectionUpdates(User domainUser, UserEntity entity)
    {   
        HandleBlockedUsers(domainUser, entity);
        HandleFollowingUsers(domainUser, entity);
        HandleSocialMedias(domainUser, entity);
        HandleWishlist(domainUser, entity);

        return Task.CompletedTask;
    }

    

    private Task HandleBlockedUsers(User domainUser, UserEntity entity)
    {
        var currentBlockedIds = domainUser.Blocked.ToHashSet();
        var existingBlockedIds = entity.BlockedUsers.Select(ub => ub.BlockedId).ToHashSet();

        // Add new entries
        foreach (var blockedId in currentBlockedIds.Except(existingBlockedIds))
        {
            entity.BlockedUsers.Add(new UserBlockedEntity { BlockerId = entity.Id, BlockedId = blockedId });
        }

        // Remove deleted entries
        var toRemove = entity
            .BlockedUsers
            .Where(ub => !currentBlockedIds.Contains(ub.BlockedId))
            .ToList();

        foreach (var item in toRemove)
            entity.BlockedUsers.Remove(item);

        return Task.CompletedTask;        
    }

    private Task HandleFollowingUsers(User domainUser, UserEntity entity)
    {
        var currentFollowingIds = domainUser.Followed.ToHashSet();
        var existingFollowingIds = entity.Following.Select(uf => uf.FollowedId).ToHashSet();

        // Add new entries
        foreach (var followingId in currentFollowingIds.Except(existingFollowingIds))
        {
            entity.Following.Add(new UserFollowingEntity { FollowerId = entity.Id, FollowedId = followingId });
        }

        // Remove deleted entries
        var toRemove = entity.Following.Where(uf => !currentFollowingIds.Contains(uf.FollowedId)).ToList();
        foreach (var item in toRemove)
            entity.Following.Remove(item);

        return Task.CompletedTask;
    }

    private Task HandleSocialMedias(User domainUser, UserEntity entity)
    {
        var currentIds = domainUser.SocialMediaLinks.Select(sml => sml.Id).ToHashSet();
        var existingIds = entity.SocialMediaLinks.Select(sml => sml.Id).ToHashSet();

        // Add new entries
        foreach (var linkId in currentIds.Except(existingIds))
        {
            var link = domainUser.SocialMediaLinks.Single(sml => sml.Id == linkId);
            var entityLink = _mapper.Map<SocialMediaLink, SocialMediaLinkEntity>(link);
            entity.SocialMediaLinks.Add(entityLink);
        }

        // Remove deleted entries
        var toRemove = entity
            .SocialMediaLinks
            .Where(e => !currentIds.Contains(e.Id))
            .ToList();

        foreach (var e in toRemove)
            entity.SocialMediaLinks.Remove(e);

        return Task.CompletedTask;
    }

    private Task HandleWishlist(User domainUser, UserEntity entity)
    {
        var currentBookIds = domainUser.Wishlist.ToHashSet();
        var existingBookIds = entity.Wishlist.Select(uw => uw.GeneralBookId).ToHashSet();

        // Add new entries
        foreach (var bookId in currentBookIds.Except(existingBookIds))
        {
            entity.Wishlist.Add(new UserWishlistEntity { UserId = entity.Id, GeneralBookId = bookId });
        }

        // Remove deleted entries
        var toRemove = entity.Wishlist.Where(uw => !currentBookIds.Contains(uw.GeneralBookId)).ToList();
        foreach (var item in toRemove)
            entity.Wishlist.Remove(item);
        
        return Task.CompletedTask;
    }
}
