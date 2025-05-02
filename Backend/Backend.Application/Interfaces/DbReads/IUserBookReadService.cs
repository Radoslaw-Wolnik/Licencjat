using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.DbReads;

public interface IUserBookReadService
{
    Task<UserBook> GetByIdAsync(Guid id);
    Task<UserBook> GetByAsync(Expression<Func<BookProjection, bool>> predicate);
    Task<UserBook> GetBookWithIncludes(Guid bookId, 
        params Expression<Func<BookProjection, object>>[] includes);
}
