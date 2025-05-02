using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;

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

    public async Task AddAsync(Swap swap)
    {
        var entity = _mapper.Map<SwapEntity>(swap);
        _db.Swaps.Add(entity);
        await _db.SaveChangesAsync();
        
        // user.SetId(entity.Id); 
    }

    public async Task UpdateAsync(Swap swap)
    {
        var existing = await _db.Swaps.FindAsync(swap.Id);
        _mapper.Map(swap, existing);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid swapId)
    {
        var existing = await _db.Swaps.FindAsync(swapId);
        if (existing is null)
            throw new KeyNotFoundException($"Swap with Id = {swapId} was not found.");

        _db.Swaps.Remove(existing);
        await _db.SaveChangesAsync();  
    }
}
