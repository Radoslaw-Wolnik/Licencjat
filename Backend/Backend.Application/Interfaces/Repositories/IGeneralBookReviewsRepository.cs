using Backend.Domain.Common;

namespace Backend.Application.Interfaces.Repositories;

public interface IGeneralBookReviewsRepository
{
    Task<IReadOnlyCollection<Bookmark>> GetByBookIdAsync(Guid bookId);

    Task AddAsync(Review review);
    Task RemoveAsync(Guid reviewId);
    Task UpdateAsync(Review review);
}