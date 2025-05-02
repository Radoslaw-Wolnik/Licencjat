using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;

namespace Backend.Infrastructure.Repositories.UserBooks;

public class UserBookBookmarkRepository : IUserBookBookmarkRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UserBookBookmarkRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<Bookmark>> GetByUserBookIdAsync(Guid userBookId)
    {
        var entities = await _db.Bookmarks
            .AsNoTracking()
            .Where(x => x.UserBookId == userBookId)
            .ToListAsync();
        return _mapper.Map<List<Bookmark>>(entities);
    }

    public async Task AddAsync(Bookmark bookmark)
    {
        var entity = _mapper.Map<BookmarkEntity>(bookmark);
        _db.Bookmarks.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Bookmark bookmark)
    {
        var existing = await _db.Bookmarks.FindAsync(bookmark.Id);
        _mapper.Map(bookmark, existing);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid bookamrkId)
    {
        var existing = await _db.Bookmarks.FindAsync(bookamrkId);
        if (existing is null)
            throw new KeyNotFoundException($"Bookmark with Id = {bookamrkId} was not found.");

        _db.Bookmarks.Remove(existing);
        await _db.SaveChangesAsync();  
    }
    
}
