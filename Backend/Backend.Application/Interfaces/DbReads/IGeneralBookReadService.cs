using Backend.Domain.Common;
using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.DbReads;

public interface IGeneralBookReadService
{
    Task<GeneralBook> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GeneralBook> GetFullByIdAsync(Guid bookId, CancellationToken cancellationToken = default);

    Task<Review> GetReviewByIdAsync(Guid reviewId, CancellationToken cancellationToken = default); // its not the best its here but we need it to update the review properly
}