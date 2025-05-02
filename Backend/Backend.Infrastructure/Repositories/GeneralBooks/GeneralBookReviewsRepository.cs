using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;

namespace Backend.Infrastructure.Repositories.GeneralBooks;

public class GeneralBookReviewsRepository : IGeneralBookReviewsRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public GeneralBookReviewsRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<Bookmark>> GetByBookIdAsync(Guid bookId)
    {
        var entities = await _db.Reviews
            .AsNoTracking()
            .Where(x => x.BookId == bookId)
            .ToListAsync();
        return _mapper.Map<List<Bookmark>>(entities);
    }

    public async Task AddAsync(Review review)
    {
        var entity = _mapper.Map<ReviewEntity>(review);
        _db.Reviews.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Review review)
    {
        var existing = await _db.Reviews.FindAsync(review.Id);
        _mapper.Map(review, existing);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid reviewId)
    {
        var existing = await _db.Reviews.FindAsync(reviewId);
        if (existing is null)
            throw new KeyNotFoundException($"Review with Id = {reviewId} was not found.");

        _db.Reviews.Remove(existing);
        await _db.SaveChangesAsync();
    }
    
}
