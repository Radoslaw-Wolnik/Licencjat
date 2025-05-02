using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;

namespace Backend.Infrastructure.Repositories.Swaps;

public class SwapTimelineRepository : ISwapTimelineRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public SwapTimelineRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db     = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<TimelineUpdate>> GetByIdAsync(Guid swapId)
    {
        var entities = await _db.Timelines
            .AsNoTracking()
            .Where(x => x.SwapId == swapId)
            .ToListAsync();
        return _mapper.Map<List<TimelineUpdate>>(entities);
    }

    public async Task AddAsync(TimelineUpdate timelineUpdate)
    {
        var entity = _mapper.Map<TimelineEntity>(timelineUpdate);
        _db.Timelines.Add(entity);
        await _db.SaveChangesAsync();
    }
    
}
