using Backend.Domain.Entities;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IWriteUserRepository
{
    Task<Result<Guid>> AddAsync(User user, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(User user, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}