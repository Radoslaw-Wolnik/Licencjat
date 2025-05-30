using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Data;
using AutoMapper;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Common;

namespace Backend.Infrastructure.Services.DbReads;

public class UserBookReadService : IUserBookReadService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserBookReadService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserBook> GetByIdAsync(
        Guid bookId,
        CancellationToken cancellationToken = default
    ) {
        var dbBook = await _context.UserBooks.FindAsync(bookId, cancellationToken);
        return _mapper.Map<UserBook>(dbBook);
    }

    public async Task<UserBook> GetFullByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) {
        var entity = await _context.UserBooks
            .Include(ub => ub.Bookmarks)
            .FirstAsync(ub => ub.Id == id, cancellationToken);

        return _mapper.Map<UserBook>(entity);
    }
    
    // only for updting a bookmark
    public async Task<Bookmark> GetBookmarkByIdAsync(
        Guid bookmarkId,
        CancellationToken cancellationToken = default
    ) {
         var entity = await _context.Bookmarks.
            FirstOrDefaultAsync(b => b.Id == bookmarkId, cancellationToken);

        return _mapper.Map<Bookmark>(entity);
    }
}