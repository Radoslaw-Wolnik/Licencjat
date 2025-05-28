using AutoMapper;
using AutoMapper.QueryableExtensions;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.Users;
using Backend.Domain.Common;
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
}