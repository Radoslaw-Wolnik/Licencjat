using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.Repositories;

public interface IWriteSwapRepository
{
    Task AddAsync(Swap swap);
    Task UpdateAsync(Swap swap);
    Task DeleteAsync(Guid swapId);
}
