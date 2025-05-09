using Backend.Domain.Entities;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IWriteSwapRepository
{
    Task<Result<Guid>> AddAsync(Swap swap, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(Swap swap, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid swapId, CancellationToken cancellationToken);
}
