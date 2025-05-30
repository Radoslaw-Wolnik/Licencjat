using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Data;
using AutoMapper;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Common;

namespace Backend.Infrastructure.Services.DbReads;

public class SwapReadService : ISwapReadService
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public SwapReadService(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Swap?> GetByIdAsync(
        Guid swapId,
        CancellationToken cancellationToken = default
    )
    {
        var swapEntity = await _db.Swaps
            // --- SubSwapRequesting navigations ---
            .Include(s => s.SubSwapRequesting)
                .ThenInclude(ss => ss.UserBookReading)
            .Include(s => s.SubSwapRequesting)
                .ThenInclude(ss => ss.Feedback)
            .Include(s => s.SubSwapRequesting)
                .ThenInclude(ss => ss.Issue)

            // --- SubSwapAccepting navigations ---
            .Include(s => s.SubSwapAccepting)
                .ThenInclude(ss => ss.UserBookReading)
            .Include(s => s.SubSwapAccepting)
                .ThenInclude(ss => ss.Feedback)
            .Include(s => s.SubSwapAccepting)
                .ThenInclude(ss => ss.Issue)

            // --- Meetups ---
            .Include(s => s.Meetups)

            // --- TimelineUpdates ---
            .Include(s => s.TimelineUpdates)

            // avoids one giant SQL with tons of JOINs
            .AsSplitQuery()

            // since we know it’s a PK lookup
            .SingleOrDefaultAsync(s => s.Id == swapId, cancellationToken);

        if (swapEntity == null) return null;

        return _mapper.Map<Swap>(swapEntity);
    }

    public async Task<Guid?> GetSubSwapId(
        Guid swapId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var subSwapEntity = await _db.SubSwaps.FirstOrDefaultAsync(ss => ss.UserId == userId && ss.SwapId == swapId, cancellationToken);
        if (subSwapEntity == null)
            return null;
        return subSwapEntity.Id;
    }
    public async Task<Meetup> GetMeetupById(
        Guid meetupId,
        CancellationToken cancellationToken = default
    ) {
        var meetupEntity = await _db.Meetups.
            FirstOrDefaultAsync(m => m.Id == meetupId, cancellationToken);
        return _mapper.Map<Meetup>(meetupEntity);
    }
}
