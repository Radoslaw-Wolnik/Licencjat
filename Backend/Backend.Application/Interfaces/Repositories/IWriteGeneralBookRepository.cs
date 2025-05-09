using Backend.Domain.Entities;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IWriteGeneralBookRepository
{
    Task<Result<Guid>> AddAsync(GeneralBook user, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(GeneralBook book, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid bookId, CancellationToken cancellationToken);
}
