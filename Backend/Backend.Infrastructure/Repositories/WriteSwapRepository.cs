using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Common;

namespace Backend.Infrastructure.Repositories.Swaps;

public class WriteSwapRepository : IWriteSwapRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public WriteSwapRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> AddAsync(Swap swap, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<SwapEntity>(swap);
        _db.Swaps.Add(entity);
        
        var result = await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add Swap");
        
        return result.IsSuccess
            ? Result.Ok(entity.Id)
            : Result.Fail<Guid>(result.Errors);
    }

    public async Task<Result> DeleteAsync(Guid swapId, CancellationToken cancellationToken)
    {
        // var existing = await _db.Swaps.FindAsync(swapId);
        var existing = await _db.Swaps.FindAsync([swapId], cancellationToken);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("Swap", swapId));

        _db.Swaps.Remove(existing);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to ddelete Swap");  
    }

    public async Task<Result> UpdateAsync(Swap swap, CancellationToken cancellationToken)
    {
        var entity = await _db.Swaps
            .Include(s => s.SubSwapRequesting)
                .ThenInclude(ss => ss.Feedback)
            .Include(s => s.SubSwapRequesting)
                .ThenInclude(ss => ss.Issue)
            .Include(s => s.SubSwapAccepting)
                .ThenInclude(ss => ss.Feedback)
            .Include(s => s.SubSwapAccepting)
                .ThenInclude(ss => ss.Issue)
            .Include(s => s.Meetups)
            .Include(s => s.TimelineUpdates)
            .FirstOrDefaultAsync(s => s.Id == swap.Id, cancellationToken);

        if (entity == null)
            return Result.Fail(DomainErrorFactory.NotFound("Swap", swap.Id));

        // Map all scalar and nested properties
        _mapper.Map(swap, entity);

        // Sync meetups
        SyncCollection(
            swap.Meetups,
            entity.Meetups,
            (d, e) => d.Id == e.Id,
            d => _mapper.Map<MeetupEntity>(d),
            (d, e) => _mapper.Map(d, e)
        );

        // Sync timeline updates
        SyncCollection(
            swap.TimelineUpdates,
            entity.TimelineUpdates,
            (d, e) => d.Id == e.Id,
            d => _mapper.Map<TimelineEntity>(d),
            (d, e) => _mapper.Map(d, e)
        );

        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to update Swap");
    }

    private void SyncCollection<TDomain, TEntity>(
        IEnumerable<TDomain> domainItems,
        ICollection<TEntity> entityItems,
        Func<TDomain, TEntity, bool> match,
        Func<TDomain, TEntity> createEntity,
        Action<TDomain, TEntity> updateEntity)
        where TEntity : class
    {
        // Remove deleted
        var toRemove = entityItems
            .Where(e => !domainItems.Any(d => match(d, e)))
            .ToList();
        foreach (var e in toRemove)
            entityItems.Remove(e);

        // Add or update
        foreach (var d in domainItems)
        {
            var existing = entityItems.FirstOrDefault(e => match(d, e));
            if (existing == null)
                entityItems.Add(createEntity(d));
            else
                updateEntity(d, existing);
        }
    }

     public async Task<Result> AddTimelineUpdateAsync(TimelineUpdate update, CancellationToken cancellationToken)
    {
        var timelineEntity = _mapper.Map<TimelineEntity>(update);
        _db.Timelines.Add(timelineEntity);

        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add a timeline update");
    }

}
