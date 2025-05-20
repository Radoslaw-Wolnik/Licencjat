using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Domain.Common;
using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.DbReads;

public interface IGeneralBookReadService
{
    Task<bool> ExistsAsync(Expression<Func<BookProjection, bool>> predicate, CancellationToken cancellationToken = default);
    Task<GeneralBook> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GeneralBook> GetByAsync(Expression<Func<BookProjection, bool>> predicate, CancellationToken cancellationToken = default);
    Task<GeneralBook> GetFullByIdAsync(Guid bookId, CancellationToken cancellationToken = default);
    
    Task<Review> GetReviewByIdAsync(Guid reviewId, CancellationToken cancellationToken = default); // its not the best its here but we need it to update the review properly
}