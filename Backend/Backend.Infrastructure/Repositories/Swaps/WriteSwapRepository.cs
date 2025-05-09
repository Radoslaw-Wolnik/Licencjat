using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;

namespace Backend.Infrastructure.Repositories.Swaps;

public class WriteSwapRepository : IWriteSwapRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public WriteSwapRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> AddAsync(Swap swap, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<SwapEntity>(swap);
        _db.Swaps.Add(entity);
        
        var result = await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add Swap");
        
        return result.IsSuccess
            ? Result.Ok(entity.Id)
            : Result.Fail<Guid>(result.Errors);
    }

    public async Task<Result> UpdateAsync(Swap swap, CancellationToken cancellationToken)
    {
        var existing = await _db.Swaps.FindAsync(swap.Id);
        _mapper.Map(swap, existing);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to update Swap");
    }

    public async Task<Result> DeleteAsync(Guid swapId, CancellationToken cancellationToken)
    {
        var existing = await _db.Swaps.FindAsync(swapId);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("Swap", swapId));

        _db.Swaps.Remove(existing);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to ddelete Swap");  
    }
}
