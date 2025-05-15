using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;

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

    public async Task<Result<Guid>> AddAsync(Review review, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ReviewEntity>(review);
        _db.Reviews.Add(entity);
        
        var result = await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add review");
        return result.IsSuccess
            ? Result.Ok(entity.Id)
            : Result.Fail<Guid>(result.Errors);
    }

    public async Task<Result> UpdateAsync(Review review, CancellationToken cancellationToken)
    {
        var existing = await _db.Reviews.FindAsync(review.Id);
        _mapper.Map(review, existing);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to update review");
    }

    public async Task<Result> RemoveAsync(Guid reviewId, CancellationToken cancellationToken)
    {
        var existing = await _db.Reviews.FindAsync(reviewId);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("Review", reviewId));

        _db.Reviews.Remove(existing);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to delete review");
    }
    
}
