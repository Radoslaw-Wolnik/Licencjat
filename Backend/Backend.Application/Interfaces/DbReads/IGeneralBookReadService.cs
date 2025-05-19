using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.DbReads;

public interface IGeneralBookReadService
{
    Task<bool> ExistsAsync(Expression<Func<BookProjection, bool>> predicate, CancellationToken cancellationToken = default);
    Task<GeneralBook> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GeneralBook> GetByAsync(Expression<Func<BookProjection, bool>> predicate, CancellationToken cancellationToken = default);
    Task<GeneralBook> GetFullByIdAsync(Guid bookId, CancellationToken cancellationToken = default);
}