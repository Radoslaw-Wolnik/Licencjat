using Backend.Domain.Common;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IGeneralBookReviewsRepository
{
    Task<IReadOnlyCollection<Review>> GetByBookIdAsync(Guid bookId);

    Task<Result<Guid>> AddAsync(Review review, CancellationToken cancellationToken);
    Task<Result> RemoveAsync(Guid reviewId, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(Review review, CancellationToken cancellationToken);
}