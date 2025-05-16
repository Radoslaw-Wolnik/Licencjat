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

    // Scalar-only update (profile, settings, etc.)
    public async Task<Result> UpdateProfileAsync(User domainUser, CancellationToken cancellationToken)
    {
        // Attach stub entity with only key
        var stub = new UserEntity { Id = domainUser.Id };
        _db.Users.Attach(stub);

        // Map scalar properties manually
        // stub.UserName = domainUser.Username;
        // stub.Email = domainUser.Email;
        stub.FirstName = domainUser.FirstName;
        stub.LastName = domainUser.LastName;
        stub.City = domainUser.Location.City;
        stub.Country = domainUser.Location.Country.Code;
        stub.ProfilePicture = domainUser.ProfilePicture?.Link;
        stub.Bio = domainUser.Bio.Value;
        stub.Reputation = domainUser.Reputation.Value;
        
        // mark only these as modified
        //_db.Entry(stub).Property(e => e.UserName).IsModified = true;
        //_db.Entry(stub).Property(e => e.Email).IsModified = true;
        _db.Entry(stub).Property(e => e.FirstName).IsModified = true;
        _db.Entry(stub).Property(e => e.LastName).IsModified = true;
        _db.Entry(stub).Property(e => e.City).IsModified = true;
        _db.Entry(stub).Property(e => e.Country).IsModified = true;
        _db.Entry(stub).Property(e => e.ProfilePicture).IsModified = true;
        _db.Entry(stub).Property(e => e.Bio).IsModified = true;
        _db.Entry(stub).Property(e => e.Reputation).IsModified = true;

        // as we marked the scalar fields as modified the EFCore will check if they changed and if so then it will be overwritten/updaten
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to update User Profile"); // domainUser.Id
    }

    // Blocked-users collection
    public async Task<Result> UpdateBlockedUsersAsync(
        Guid userId,
        IEnumerable<Guid> allBlockedUserIds,
        CancellationToken cancellationToken)
    {
        var entity = await _db.Users
            .Include(u => u.BlockedUsers)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (entity == null)
            return Result.Fail(DomainErrorFactory.NotFound("User", userId));

        var current = allBlockedUserIds.ToHashSet();
        var existing = entity.BlockedUsers.Select(b => b.BlockedId).ToHashSet();

        // Add new
        foreach (var id in current.Except(existing))
            entity.BlockedUsers.Add(new UserBlockedEntity { BlockerId = userId, BlockedId = id });

        // Remove missing
        var toRemove = entity.BlockedUsers.Where(b => !current.Contains(b.BlockedId)).ToList();
        toRemove.ForEach(b => entity.BlockedUsers.Remove(b));

        return await _db.SaveChangesWithResultAsync(cancellationToken, "failed to update the users that are blocked"); // userId
    }
    public async Task<Result> AddBlockedUserAsync(Guid userId, Guid blockedId, CancellationToken cancellationToken)
    {
        // var stub = new UserEntity { Id = userId };
        // _db.Users.Attach(stub);
        // then
        // stub.UserBlocked.Add(...);
        _db.UserBlockeds.Add(new UserBlockedEntity {
            BlockerId = userId,
            BlockedId = blockedId
        });
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add User to blocked users"); // userId
    }

    // Following-users collection
    public async Task<Result> UpdateFollowingUsersAsync(
        Guid userId,
        IEnumerable<Guid> allFollowedUserIds,
        CancellationToken cancellationToken)
    {
        var entity = await _db.Users
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (entity == null)
            return Result.Fail(DomainErrorFactory.NotFound("User", userId));

        var current = allFollowedUserIds.ToHashSet();
        var existing = entity.Following.Select(f => f.FollowedId).ToHashSet();

        foreach (var id in current.Except(existing))
            entity.Following.Add(new UserFollowingEntity { FollowerId = userId, FollowedId = id });

        var toRemove = entity.Following.Where(f => !current.Contains(f.FollowedId)).ToList();
        toRemove.ForEach(f => entity.Following.Remove(f));

        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to update the list of followed users"); // userId
    }

    public async Task<Result> AddFollowingUserAsync(Guid userId, Guid followingId, CancellationToken cancellationToken)
    {
        _db.UserFollowings.Add(new UserFollowingEntity {
            FollowerId = userId,
            FollowedId = followingId
        });
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add User to followed users"); // userId
    }

    // Social media links collection
    public async Task<Result> UpdateSocialMediasAsync(
        Guid userId,
        IEnumerable<SocialMediaLink> links,
        CancellationToken cancellationToken)
    {
        var entity = await _db.Users
            .Include(u => u.SocialMediaLinks)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (entity == null)
            return Result.Fail(DomainErrorFactory.NotFound("User", userId));

        var currentIds = links.Select(l => l.Id).ToHashSet();
        var existingIds = entity.SocialMediaLinks.Select(e => e.Id).ToHashSet();

        // Add new
        foreach (var id in currentIds.Except(existingIds))
        {
            var link = links.Single(l => l.Id == id);
            entity.SocialMediaLinks.Add(_mapper.Map<SocialMediaLinkEntity>(link));
        }

        // Remove missing
        var toRemove = entity.SocialMediaLinks.Where(e => !currentIds.Contains(e.Id)).ToList();
        toRemove.ForEach(e => entity.SocialMediaLinks.Remove(e));

        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to update Social Media Link of the User"); // userId
    }

    public async Task<Result> AddSocialMediaAsync(Guid userId, SocialMediaLink socialMediaLink, CancellationToken cancellationToken)
    {
        var socialEntity = _mapper.Map<SocialMediaLinkEntity>(socialMediaLink);
        _db.SocialMediaLinks.Add(socialEntity);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add SocialMediaLink"); // userId
    }

    // Wishlist collection
    public async Task<Result> UpdateWishlistAsync(
        Guid userId,
        IEnumerable<Guid> allWishlistedBookIds,
        CancellationToken cancellationToken)
    {
        var entity = await _db.Users
            .Include(u => u.Wishlist)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (entity == null)
            return Result.Fail(DomainErrorFactory.NotFound("User", userId));

        var current = allWishlistedBookIds.ToHashSet();
        var existing = entity.Wishlist.Select(w => w.GeneralBookId).ToHashSet();

        foreach (var id in current.Except(existing))
            entity.Wishlist.Add(new UserWishlistEntity { UserId = userId, GeneralBookId = id });

        var toRemove = entity.Wishlist.Where(w => !current.Contains(w.GeneralBookId)).ToList();
        toRemove.ForEach(w => entity.Wishlist.Remove(w));

        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to update Wishlist of the User"); // userId
    }

    public async Task<Result> AddWishlistBookAsync(Guid userId, Guid wishlistBookId, CancellationToken cancellationToken)
    {
        _db.UserWishlists.Add(new UserWishlistEntity {
            UserId        = userId,
            GeneralBookId = wishlistBookId
        });
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add book to wishlist for the user"); // userId
    }
}
