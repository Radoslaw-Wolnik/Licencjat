using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.DbReads;

public interface ISwapReadService
{
    Task<Swap?> GetByIdAsync(Guid swapId);
}
