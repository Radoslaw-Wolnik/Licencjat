using Backend.Domain.Common;
using Backend.Domain.Enums.SortBy;
using Backend.Application.ReadModels.Users;
using Backend.Application.ReadModels.Common;
using Backend.Domain.Enums;

namespace Backend.Application.Interfaces.Queries;

public interface IUserQueryService
{

    Task<PaginatedResult<UserSmallReadModel>> ListAsync(
        string UserName,
        float Reputation,

        string City,
        string Country,

        SortUsersBy sortBy,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default
    );

    Task<UserProfileReadModel?> GetDetailsAsync(
        Guid userId,
        CancellationToken ct = default
    );

    // children collections:
    Task<SocialMediaLinkReadModel?> GetSocialMediaByIdAsync(
        Guid socialMediaId, CancellationToken ct = default);

    Task<PaginatedResult<UserSmallReadModel>> ListBlockedAsync(
        Guid userId,
        string? usernameFilter,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default);
        
    Task<PaginatedResult<UserSmallReadModel>> ListFollowedAsync(
        Guid userId,
        string? usernameFilter,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default);

    Task<PaginatedResult<UserSmallReadModel>> ListFollowersAsync(
        Guid userId,
        string? usernameFilter,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default);

    Task<IReadOnlyCollection<SocialMediaLinkReadModel>> ListSocialMediaAsync(
        Guid userId,
        CancellationToken ct = default);

    Task<PaginatedResult<BookCoverItemReadModel>> ListWishlistAsync(
        Guid userId,
        string? titleFilter,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default);

}
