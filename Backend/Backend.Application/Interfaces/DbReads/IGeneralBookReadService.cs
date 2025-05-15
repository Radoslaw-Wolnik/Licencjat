using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.DbReads;

public interface IGeneralBookReadService
{
    Task<bool> ExistsAsync(Expression<Func<BookProjection, bool>> predicate);
    Task<GeneralBook> GetByIdAsync(Guid id);
    Task<GeneralBook> GetByAsync(Expression<Func<BookProjection, bool>> predicate);
    Task<GeneralBook> GetFullByIdAsync(Guid bookId);
}