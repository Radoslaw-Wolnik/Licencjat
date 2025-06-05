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
using Backend.Infrastructure.Mapping;
using Backend.Domain.Enums;

namespace Backend.Infrastructure.Repositories;

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
        // add timeline entity
        var timelineEntity = _mapper.Map<TimelineEntity>(update);
        _db.Timelines.Add(timelineEntity);

        // attach stub for swap to change the status to the status of last timelineUpdate
        var tempSwap = new SwapEntity { Id = update.SwapId };
        _db.Swaps.Attach(tempSwap);

        tempSwap.Status = _mapper.Map<SwapStatus>(update.Status);
        
        var entry = _db.Entry(tempSwap);
        entry.Property(e => e.Status).IsModified = true;

        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add a timeline update");
    }


    public async Task<Result> AddFeedbackAsync(
        Feedback feedback,
        CancellationToken cancellationToken
    ) {
        // map and add new feedback entity
        var feedbackEntity = _mapper.Map<FeedbackEntity>(feedback);
        _db.Feedbacks.Add(feedbackEntity);

        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add a new feedback");
    }
    
    public async Task<Result> AddIssueAsync(
        Issue issue,
        CancellationToken cancellationToken
    ) {
        // map and add new issue entity
        var issueEntity = _mapper.Map<IssueEntity>(issue);
        _db.Issues.Add(issueEntity);

        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add a new issue");
    }
    
    public async Task<Result> RemoveIssueAsync(
        Guid issueId,
        CancellationToken cancellationToken
    ) {
        var iss = await _db.Issues.FindAsync([issueId], cancellationToken);
        if (iss == null)
            return Result.Fail(DomainErrorFactory.NotFound("Meetup", issueId));

        _db.Issues.Remove(iss);

        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to remove the issue");
    }
    
    public async Task<Result> AddMeetupAsync(
        Meetup meetup,
        CancellationToken cancellationToken
    ) {
        // map and add new meetup entity
        var meetupEntity = _mapper.Map<MeetupEntity>(meetup);
        _db.Meetups.Add(meetupEntity);

        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add a new meetup");
    }
    
    public async Task<Result> RemoveMeetupAsync(
        Guid meetupId,
        CancellationToken cancellationToken
    )
    {
        var mee = await _db.Meetups.FindAsync([meetupId], cancellationToken);
        if (mee == null)
            return Result.Fail(DomainErrorFactory.NotFound("Meetup", meetupId));

        _db.Meetups.Remove(mee);

        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to remove the meetup");
    }

    public async Task<Result> UpdateMeetupAsync(
        Meetup updated,
        CancellationToken cancellationToken
    ) {
        //Try to find a local (already-tracked) entity with the same Id
        var localEntry = _db.ChangeTracker
                                .Entries<MeetupEntity>()
                                .FirstOrDefault(e => e.Entity.Id == updated.Id);

        MeetupEntity stub;

        if (localEntry != null)
        {
            // The context is already tracking that Id, so use the existing instance
            stub = localEntry.Entity;
        }
        else
        {
            // Not tracked yet: create a "detached" stub and Attach it
            stub = new MeetupEntity { Id = updated.Id };
            _db.Meetups.Attach(stub);
        }
        
        stub.Status = updated.Status;
        stub.Location_X = (float)updated.Location.Latitude;
        stub.Location_Y = (float)updated.Location.Longitude;
        
        var entry = _db.Entry(stub);
        entry.Property(e => e.Status).IsModified = true;
        entry.Property(e => e.Location_X).IsModified  = true;
        entry.Property(e => e.Location_Y).IsModified = true;
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to update the meetup");
    }
}

