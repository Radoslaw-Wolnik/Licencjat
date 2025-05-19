using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.DbReads;

public interface IUserBookReadService
{
    Task<UserBook> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserBook> GetByAsync(Expression<Func<BookProjection, bool>> predicate, CancellationToken cancellationToken = default);
    Task<UserBook> GetFullByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
