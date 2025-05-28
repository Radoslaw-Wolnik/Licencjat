using Backend.Domain.Enums;
using Backend.Domain.Common;
using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Enums.SortBy;
using System.Linq.Expressions;

namespace Backend.Application.Interfaces.Queries;

public interface IGeneralBookQueryService
{
    /// <summary>
    /// Returns a paged list of GeneralBookListItems for general books
    /// </summary>
    Task<PaginatedResult<GeneralBookListItem>> ListAsync(
        string? titleFilter,
        string? authorFilter,
        BookGenre? genreFilter,
        SortGeneralBookBy sortBy,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default
    );

    /// <summary>
    /// Returns details for a single book including up to maxReviews
    /// </summary>
    Task<GeneralBookDetailsReadModel?> GetBookDetailsAsync(
        Guid bookId,
        int maxReviews = 10,
        CancellationToken ct = default
    );

    Task<PaginatedResult<ReviewReadModel>> GetPaginatedReviewsAsync(
        Guid bookId,
        SortReviewsBy sortBy,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default);
}
