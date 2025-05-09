using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using Backend.Infrastructure.Extensions;
using FluentResults;

namespace Backend.Infrastructure.Repositories.Swaps;

public class SwapFeedbackRepository : ISwapFeedbackRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public SwapFeedbackRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db     = db;
        _mapper = mapper;
    }

    public async Task<Feedback> GetByIdAsync(Guid subSwapId)
    {
        var entities = await _db.Feedbacks
            .AsNoTracking()
            .Where(x => x.SubSwapId == subSwapId)
            .FirstOrDefaultAsync();
        return _mapper.Map<Feedback>(entities);
    }

    public async Task<Result<Guid>> AddAsync(Feedback feedback, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<FeedbackEntity>(feedback);
        _db.Feedbacks.Add(entity);

        var result = await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add feedback to swap");
        return result.IsSuccess
            ? Result.Ok(entity.Id)
            : Result.Fail<Guid>(result.Errors);
    }

    public async Task<Result> UpdateAsync(Feedback feedback, CancellationToken cancellationToken)
    {
        var existing = await _db.Feedbacks.FindAsync(feedback.Id);
        _mapper.Map(existing, feedback);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to update feedback");
    }

    public async Task<Result> RemoveAsync(Guid feedbackId, CancellationToken cancellationToken)
    {
        var existing = await _db.Feedbacks.FindAsync(feedbackId);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("Feedback", feedbackId));

        _db.Feedbacks.Remove(existing);

        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to delete feedback");
    }
}
