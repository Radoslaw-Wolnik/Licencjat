using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Data;
using AutoMapper;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.DbReads;

namespace Backend.Infrastructure.Services.DbReads;

public class SwapReadService : ISwapReadService
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public SwapReadService(ApplicationDbContext db, IMapper mapper)
    {
        _db     = db;
        _mapper = mapper;
    }

    public async Task<Swap?> GetByIdAsync(Guid swapId)
    {
        var swapEntity = await _db.Swaps
            .Where(s => s.Id == swapId)
            .Include(s => s.SubSwapRequesting)
              .ThenInclude(ss => ss.Feedback)
            .Include(s => s.SubSwapRequesting)
              .ThenInclude(ss => ss.Issue)
            .Include(s => s.SubSwapAccepting)
              .ThenInclude(ss => ss.Feedback)
            .Include(s => s.SubSwapAccepting)
              .ThenInclude(ss => ss.Issue)
            .Include(s => s.Meetups)
            .Include(s => s.TimelineUpdates)
            .SingleOrDefaultAsync();
            // FIndasync is optimised for Primary keys but it doesnt work with .include's
            // .FindAsync(swapId)

        var domainSwap = _mapper.Map<Swap>(swapEntity);

        return domainSwap;
    }

}
