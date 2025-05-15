using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;

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

    public async Task<Result<Guid>> AddAsync(Bookmark bookmark, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<BookmarkEntity>(bookmark);
        _db.Bookmarks.Add(entity);
        var result = await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add Bookmark");
        return result.IsSuccess
            ? Result.Ok(entity.Id)
            : Result.Fail<Guid>(result.Errors);
    }

    public async Task<Result> UpdateAsync(Bookmark bookmark, CancellationToken cancellationToken)
    {
        var existing = await _db.Bookmarks.FindAsync(bookmark.Id);
        _mapper.Map(bookmark, existing);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to update Bookmark");
    }

    public async Task<Result> RemoveAsync(Guid bookamrkId, CancellationToken cancellationToken)
    {
        var existing = await _db.Bookmarks.FindAsync(bookamrkId);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("Bookmark", bookamrkId));

        _db.Bookmarks.Remove(existing);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to delete Bookmark");  
    }
    
}
