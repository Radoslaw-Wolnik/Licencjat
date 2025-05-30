using Backend.Domain.Common;
using Backend.Domain.Entities;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IWriteUserRepository
{
    Task<Result<Guid>> AddAsync(User user, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> UpdateProfileAsync(User user, CancellationToken cancellationToken);

    // child collections
    Task<Result> UpdateBlockedUsersAsync(Guid userId, IEnumerable<Guid> allBlockedUserIds, CancellationToken cancellationToken);
    Task<Result> AddBlockedUserAsync(Guid userId, Guid blockedId, CancellationToken cancellationToken);
    Task<Result> RemoveBlockedUserAsync(Guid userId, Guid blockedId, CancellationToken cancellationToken);

    Task<Result> UpdateFollowingUsersAsync(Guid userId, IEnumerable<Guid> allFollowedUserIds, CancellationToken cancellationToken);
    Task<Result> AddFollowingUserAsync(Guid userId, Guid followingId, CancellationToken cancellationToken);
    Task<Result> RemoveFollowingUserAsync(Guid userId, Guid followingId, CancellationToken cancellationToken);

    Task<Result> UpdateSocialMediasAsync(Guid userId, IEnumerable<SocialMediaLink> links, CancellationToken cancellationToken);
    Task<Result> AddSocialMediaAsync(Guid userId, SocialMediaLink socialMediaLink, CancellationToken cancellationToken);
    Task<Result> RemoveSocialMediaAsync(Guid socialMediaLinkId, CancellationToken cancellationToken);
    Task<Result> UpdateSingleSocialMediaAsync(SocialMediaLink socialMedia, CancellationToken cancellationToken);

    Task<Result> UpdateWishlistAsync(Guid userId, IEnumerable<Guid> allWishlistedBookIds, CancellationToken cancellationToken);
    Task<Result> AddWishlistBookAsync(Guid userId, Guid wishlistBookId, CancellationToken cancellationToken);
    Task<Result> RemoveWishlistBookAsync(Guid userId, Guid wishlistBookId, CancellationToken cancellationToken);
}   
