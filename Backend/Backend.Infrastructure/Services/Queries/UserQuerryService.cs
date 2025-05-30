using AutoMapper;
using AutoMapper.QueryableExtensions;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.Users;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Enums.SortBy;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Services.Queries;

public class UserQueryService : IUserQueryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserQueryService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<UserSmallReadModel>> ListAsync(
        string userName,
        float reputation,
        string city,
        string country,
        SortUsersBy sortBy,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default)
    {
        var query = _context.Users
            .AsNoTracking()
            .Include(u => u.SubSwaps)
                .ThenInclude(ss => ss.Swap)
            .ProjectTo<UserSmallReadModel>(
                _mapper.ConfigurationProvider,
                parameters: new { IncludeDetails = true } // make automapper fetch detailed SmallUser
            );

        // Apply filters
        if (!string.IsNullOrWhiteSpace(userName))
            query = query.Where(u => u.Username.Contains(userName));
        if (reputation > 0)
            query = query.Where(u => u.UserReputation >= reputation);
        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(u => u.City!.Contains(city));
        if (!string.IsNullOrWhiteSpace(country))
            query = query.Where(u => u.Country!.Contains(country));

        // Apply sorting
        query = sortBy switch
        {
            SortUsersBy.UserName => descending
                ? query.OrderByDescending(u => u.Username)
                : query.OrderBy(u => u.Username),
            SortUsersBy.UserReputation => descending
                ? query.OrderByDescending(u => u.UserReputation)
                : query.OrderBy(u => u.UserReputation),
            SortUsersBy.SwapCount => descending
                ? query.OrderByDescending(u => u.SwapCount)
                : query.OrderBy(u => u.SwapCount),
            _ => descending
                ? query.OrderByDescending(u => u.Username)
                : query.OrderBy(u => u.Username)
        };

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync(ct);

        return new PaginatedResult<UserSmallReadModel>(items, total);
    }

    public async Task<UserProfileReadModel?> GetDetailsAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.SocialMediaLinks)
            .Include(u => u.Wishlist)
                .ThenInclude(w => w.GeneralBook)
            .Include(u => u.UserBooks)
                .ThenInclude(ub => ub.Book)
            .Include(u => u.SubSwaps)
                .ThenInclude(ss => ss.Swap)
            .ProjectTo<UserProfileReadModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
    }

    // children collections
    public async Task<SocialMediaLinkReadModel?> GetSocialMediaByIdAsync(
        Guid socialMediaId, 
        CancellationToken ct = default)
    {
        return await _context.SocialMediaLinks
            .AsNoTracking()
            .Where(s => s.Id == socialMediaId)
            .ProjectTo<SocialMediaLinkReadModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<PaginatedResult<UserSmallReadModel>> ListBlockedAsync(
        Guid userId,
        string? usernameFilter,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default)
    {
        var query = _context.UserBlockeds
            .AsNoTracking()
            .Where(b => b.BlockerId == userId)
            .Select(b => b.Blocked)
            .ProjectTo<UserSmallReadModel>(_mapper.ConfigurationProvider);

        return await ExecuteUserListQuery(query, usernameFilter, descending, offset, limit, ct);
    }

    public async Task<PaginatedResult<UserSmallReadModel>> ListFollowedAsync(
        Guid userId,
        string? usernameFilter,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default)
    {
        var query = _context.UserFollowings
            .AsNoTracking()
            .Where(f => f.FollowerId == userId)
            .Select(f => f.Followed)
            .ProjectTo<UserSmallReadModel>(_mapper.ConfigurationProvider);

        return await ExecuteUserListQuery(query, usernameFilter, descending, offset, limit, ct);
    }

    public async Task<PaginatedResult<UserSmallReadModel>> ListFollowersAsync(
        Guid userId,
        string? usernameFilter,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default)
    {
        var query = _context.UserFollowings
            .AsNoTracking()
            .Where(f => f.FollowedId == userId)
            .Select(f => f.Follower)
            .ProjectTo<UserSmallReadModel>(_mapper.ConfigurationProvider);

        return await ExecuteUserListQuery(query, usernameFilter, descending, offset, limit, ct);
    }

    public async Task<IReadOnlyCollection<SocialMediaLinkReadModel>> ListSocialMediaAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        return await _context.SocialMediaLinks
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderBy(s => s.Platform) // Optional: Order by platform
            .ProjectTo<SocialMediaLinkReadModel>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<PaginatedResult<BookCoverItemReadModel>> ListWishlistAsync(
        Guid userId,
        string? titleFilter,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default)
    {
        var query = _context.UserWishlists
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .Select(w => w.GeneralBook)
            .ProjectTo<BookCoverItemReadModel>(_mapper.ConfigurationProvider);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(titleFilter))
            query = query.Where(b => b.Title.Contains(titleFilter));

        // Apply sorting
        query = descending 
            ? query.OrderByDescending(b => b.Title)
            : query.OrderBy(b => b.Title);

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync(ct);

        return new PaginatedResult<BookCoverItemReadModel>(items, total);
    }

    private async Task<PaginatedResult<UserSmallReadModel>> ExecuteUserListQuery(
        IQueryable<UserSmallReadModel> query,
        string? usernameFilter,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct)
    {
        // Apply username filter
        if (!string.IsNullOrWhiteSpace(usernameFilter))
            query = query.Where(u => u.Username.Contains(usernameFilter));

        // Apply sorting
        query = descending 
            ? query.OrderByDescending(u => u.Username)
            : query.OrderBy(u => u.Username);

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync(ct);

        return new PaginatedResult<UserSmallReadModel>(items, total);
    }
}