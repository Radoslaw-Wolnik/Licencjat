using Backend.Domain.Common;
using Backend.Domain.Entities;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IWriteGeneralBookRepository
{
    Task<Result<Guid>> AddAsync(GeneralBook user, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid bookId, CancellationToken cancellationToken);
    Task<Result> UpdateScalarsAsync(GeneralBook book, CancellationToken cancellationToken);
    
    // reviews child collection
    Task<Result> AddReviewAsync(Guid generalBookId, Review review, CancellationToken cancellationToken);
    Task<Result> UpdateReviewAsync(Review review, CancellationToken cancellationToken);
    Task<Result> RemoveReviewAsync(Guid reviewId, CancellationToken cancellationToken);
}
