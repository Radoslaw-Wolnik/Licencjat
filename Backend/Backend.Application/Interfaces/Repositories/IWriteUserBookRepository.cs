using Backend.Domain.Entities;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IWriteUserBookRepository
{
    Task<Result<Guid>> AddAsync(UserBook book, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(UserBook book, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid bookId, CancellationToken cancellationToken);
}