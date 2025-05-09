using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;

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

    public async Task<Result<Guid>> AddAsync(TimelineUpdate timelineUpdate, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<TimelineEntity>(timelineUpdate);
        _db.Timelines.Add(entity);
        
        var result = await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add TimelineUpdate");
        return result.IsSuccess
            ? Result.Ok(entity.Id)
            : Result.Fail<Guid>(result.Errors);
    }
    
}
