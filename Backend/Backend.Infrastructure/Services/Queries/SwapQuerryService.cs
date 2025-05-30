using AutoMapper;
using AutoMapper.QueryableExtensions;
using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Swaps;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Services.Queries;

public class SwapQueryService : ISwapQueryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public SwapQueryService(
        ApplicationDbContext context,
        IMapper mapper,
        IUserContext userContext)
    {
        _context = context;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<PaginatedResult<SwapListItem>> ListAsync(
        Guid userId,
        SwapStatus status,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default)
    {
        var query = _context.Swaps
            .AsNoTracking()
            .Where(s => s.Status == status &&
                (s.SubSwapRequesting.UserId == userId || s.SubSwapAccepting.UserId == userId))
            .ProjectTo<SwapListItem>(_mapper.ConfigurationProvider, new { UserId = userId });

        return await ExecutePaginatedQuery(query, descending, offset, limit, ct);
    }

    public async Task<SwapDetailsReadModel?> GetDetailsAsync(
        Guid swapId,
        int maxUpdates = 10,
        CancellationToken ct = default)
    {
        // var currentUserId = _userContext.UserId;
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException();

        return await _context.Swaps
            .AsNoTracking()
            .Include(s => s.SubSwapRequesting.User.SocialMediaLinks)
            .Include(s => s.SubSwapAccepting.User.SocialMediaLinks)
            .Include(s => s.TimelineUpdates)
            .Where(s => s.Id == swapId)
            .ProjectTo<SwapDetailsReadModel>(
                _mapper.ConfigurationProvider,
                parameters: new
                {
                    MaxUpdates = maxUpdates,
                    CurrentUserId = currentUserId
                })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<PaginatedResult<TimelineUpdateReadModel>> ListTimelineUpdateAsync(
        Guid swapId,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default)
    {
        var query = _context.Timelines
            .AsNoTracking()
            .Where(t => t.SwapId == swapId)
            .Include(t => t.User)
            .ProjectTo<TimelineUpdateReadModel>(_mapper.ConfigurationProvider);

        return await ExecutePaginatedQuery(query, descending, offset, limit, ct);
    }

    private async Task<PaginatedResult<T>> ExecutePaginatedQuery<T>(
        IQueryable<T> query,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct)
    {
        var orderedQuery = descending
            ? query.OrderByDescending(x => EF.Property<DateTime>(x!, "CreatedAt"))
            : query.OrderBy(x => EF.Property<DateTime>(x!, "CreatedAt"));

        var total = await query.CountAsync(ct);
        var results = await orderedQuery
            .Skip(offset)
            .Take(limit)
            .ToListAsync(ct);

        return new PaginatedResult<T>(results, total);
    }
    
    public async Task<FeedbackReadModel?> GetFeedbackByIdAsync(
        Guid feedbackId, 
        CancellationToken ct = default)
    {
        return await _context.Feedbacks
            .AsNoTracking()
            .Where(f => f.Id == feedbackId)
            .ProjectTo<FeedbackReadModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IssueReadModel?> GetIssueByIdAsync(
        Guid issueId, 
        CancellationToken ct = default)
    {
        return await _context.Issues
            .AsNoTracking()
            .Where(i => i.Id == issueId)
            .ProjectTo<IssueReadModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<MeetupReadModel?> GetMeetupByIdAsync(
        Guid meetupId, 
        CancellationToken ct = default)
    {
        return await _context.Meetups
            .AsNoTracking()
            .Where(m => m.Id == meetupId)
             // .Include(m => m.Swap) // Include related data as needed
             // .Include(m => m.Location)
            .ProjectTo<MeetupReadModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<PaginatedResult<MeetupReadModel>> ListMeetupsAsync(
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default)
    {
        var currentUserId = _userContext.UserId 
            ?? throw new UnauthorizedAccessException();
        
        var query = _context.Meetups
            .AsNoTracking()
            .Where(m => m.Swap.SubSwapRequesting.UserId == currentUserId || 
                    m.Swap.SubSwapAccepting.UserId == currentUserId)
            .ProjectTo<MeetupReadModel>(_mapper.ConfigurationProvider);

        return await ExecutePaginatedQuery(query, descending, offset, limit, ct);
    }
}