using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Data;
using AutoMapper;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Common;

namespace Backend.Infrastructure.Services.DbReads;

public class GeneralBookReadService : IGeneralBookReadService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GeneralBookReadService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GeneralBook> GetByIdAsync(
        Guid bookId,
        CancellationToken cancellationToken = default
    )
    {
        var dbBook = await _context.GeneralBooks.FindAsync(bookId, cancellationToken);
        //var dbBook = await _context.GeneralBooks.FindAsync([bookId, cancellationToken], cancellationToken: cancellationToken);
        return _mapper.Map<GeneralBook>(dbBook);
    }

    public async Task<GeneralBook> GetFullByIdAsync(
        Guid bookId,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await _context.GeneralBooks
            .Include(b => b.Reviews)
            .FirstAsync(b => b.Id == bookId, cancellationToken);

        return _mapper.Map<GeneralBook>(entity);
    }

    public async Task<Review> GetReviewByIdAsync(
        Guid reviewId,
        CancellationToken cancellationToken
    )
    {
        var entity = await _context.Reviews.
            FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);

        return _mapper.Map<Review>(entity);
    }
}