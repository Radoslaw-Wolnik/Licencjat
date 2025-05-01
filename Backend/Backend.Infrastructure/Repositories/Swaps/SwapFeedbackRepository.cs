using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces;

namespace Backend.Infrastructure.Repositories.Swaps;

public interface ISwapFeedbackRepository
{
    Task<Feedback> GetByIdAsync(Guid swapId);

    Task AddAsync(Feedback feedback);
    Task RemoveAsync(Guid feedbackId);
    Task UpdateAsync(Feedback feedback);
}


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

    public async Task AddAsync(Feedback feedback)
    {
        var entity = _mapper.Map<FeedbackEntity>(feedback);
        _db.Feedbacks.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid feedbackId)
    {
        var existing = await _db.Feedbacks.FindAsync(feedbackId);
        if (existing is null)
            throw new KeyNotFoundException($"Feedback with Id = {feedbackId} was not found.");

        _db.Feedbacks.Remove(existing);
    }

    public async Task UpdateAsync(Feedback feedback)
    {
        var existing = await _db.Feedbacks.FindAsync(feedback.Id);
        _mapper.Map(existing, feedback);
        await _db.SaveChangesAsync();
    }
    
}
