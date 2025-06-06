using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Common;

namespace Backend.Infrastructure.Repositories;

public class WriteUserBookRepository : IWriteUserBookRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public WriteUserBookRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<Result<Guid>> AddAsync(UserBook book, CancellationToken cancellationToken)
    {
        var dbBook = _mapper.Map<UserBookEntity>(book);
        _context.UserBooks.Add(dbBook);
        var result = await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to add UserBook");
        return result.IsSuccess
            ? Result.Ok(dbBook.Id)
            : Result.Fail<Guid>(result.Errors);
    }

    public async Task<Result> DeleteAsync(Guid bookId, CancellationToken cancellationToken)
    {
        var existing = await _context.UserBooks.FindAsync(bookId);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("UserBook", bookId));

        _context.UserBooks.Remove(existing);
        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to delete UserBook");
    }

    // Scalar-only update (no touching bookmarks)
    public async Task<Result> UpdateScalarsAsync(UserBook book, CancellationToken cancellationToken)
    {
        //Try to find a local (already-tracked) entity with the same Id
        var localEntry = _context.ChangeTracker
                                .Entries<UserBookEntity>()
                                .FirstOrDefault(e => e.Entity.Id == book.Id);

        UserBookEntity stub;

        if (localEntry != null)
        {
            // The context is already tracking that Id, so use the existing instance
            stub = localEntry.Entity;
        }
        else
        {
            // Not tracked yet: create a "detached" stub and Attach it
            stub = new UserBookEntity { Id = book.Id };
            _context.UserBooks.Attach(stub);
        }

        // Map individual scalar properties
        stub.Status = book.Status;
        stub.State = book.State;
        // stub.Language   = book.Language.Code; // idk if this one should be able to be updated
        // stub.PageCount  = book.PageCount; // same with this one, its not liek you can change the number of pages of a book
        stub.CoverPhoto = book.CoverPhoto.Link;

        // Mark as modified
        var entry = _context.Entry(stub);
        entry.Property(e => e.Status).IsModified = true;
        entry.Property(e => e.State).IsModified = true;
        // entry.Property(e => e.Language).IsModified   = true;
        // entry.Property(e => e.PageCount).IsModified  = true;
        entry.Property(e => e.CoverPhoto).IsModified = true;

        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to Update UserBook");
    }

    // Full sync of bookmarks - update whole collection
    public async Task<Result> UpdateBookmarksAsync(Guid bookId, IEnumerable<Bookmark> domainBookmarks, CancellationToken cancellationToken)
    {
        var entity = await _context.UserBooks
            .Include(ub => ub.Bookmarks)
            .FirstOrDefaultAsync(ub => ub.Id == bookId, cancellationToken);

        if (entity == null)
            return Result.Fail(DomainErrorFactory.NotFound("UserBook", bookId));

        // Remove deleted
        var byId = domainBookmarks.ToDictionary(b => b.Id);
        foreach (var existing in entity.Bookmarks.ToList())
        {
            if (!byId.ContainsKey(existing.Id))
                entity.Bookmarks.Remove(existing);
        }

        // Add or update
        var existingDict = entity.Bookmarks.ToDictionary(b => b.Id);
        foreach (var dom in domainBookmarks)
        {
            if (existingDict.TryGetValue(dom.Id, out var ent))
            {
                //_mapper.Map(dom, ent);
                ent.Colour = dom.Colour;
                ent.Page = dom.Page;
                ent.Description = dom.Description;
            }
            else
            {
                /*
                var newEntity = new BookmarkEntity
                {
                    Id = dom.Id,
                    UserBookId = dom.UserBookId,    // make sure to set the FK so EF knows its parent
                    Colour = dom.Colour,
                    Page = dom.Page,
                    Description = dom.Description
                };
                */
                var newEntity = _mapper.Map<BookmarkEntity>(dom);
                entity.Bookmarks.Add(newEntity);
                _context.Entry(newEntity).State = EntityState.Added;
                // for some reason sometimes EF Core sets new entites as Modified insted of New
                // and then it throws error as it cant update an entity becouse there is no Old one to update
                // and it throws: "Attempted to update or delete an entity that does not exist in the store."
            }
        }

        foreach (var entry in _context.ChangeTracker.Entries<BookmarkEntity>())
        {
            Console.WriteLine(
                $"▶︎ BookmarkEntity Id={entry.Entity.Id} | " +
                $"UserBookId={entry.Entity.UserBookId} | " +
                $"State={entry.State}"
            );
        }

        return await _context.SaveChangesWithResultAsync(cancellationToken, "Fialed to update Bookmarks");
    }

    // Add single bookmark (without updating the whole collection)
    public async Task<Result> AddBookmarkAsync(Bookmark bookmark, CancellationToken cancellationToken)
    {
        var bookmarkEntity = _mapper.Map<BookmarkEntity>(bookmark);
        _context.Bookmarks.Add(bookmarkEntity);

        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to add the bookmark");
    }

    public async Task<Result> RemoveBookmarkAsync(Guid bookmarkId, CancellationToken cancellationToken)
    {
        var bookmarkEntity = await _context.Bookmarks.FindAsync([bookmarkId], cancellationToken);
        if (bookmarkEntity == null)
            return Result.Fail(DomainErrorFactory.NotFound("Bookmark", bookmarkId));

        _context.Bookmarks.Remove(bookmarkEntity);

        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to add the bookmark");
    }

    public async Task<Result> UpdateBookmarkAsync(Bookmark updated, CancellationToken cancellationToken)
    {
        //Try to find a local (already-tracked) entity with the same Id
        var localEntry = _context.ChangeTracker
                                .Entries<BookmarkEntity>()
                                .FirstOrDefault(e => e.Entity.Id == updated.Id);

        BookmarkEntity stub;

        if (localEntry != null)
        {
            // The context is already tracking that Id, so use the existing instance
            stub = localEntry.Entity;
        }
        else
        {
            // Not tracked yet: create a "detached" stub and Attach it
            stub = new BookmarkEntity { Id = updated.Id };
            _context.Bookmarks.Attach(stub);
        }
        stub.Colour = updated.Colour;
        stub.Page = updated.Page;
        stub.Description = updated.Description;

        var entry = _context.Entry(stub);
        entry.Property(e => e.Colour).IsModified  = true;
        entry.Property(e => e.Page).IsModified = true;
        entry.Property(e => e.Description).IsModified = true;

        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to update the bookmark");
    }

}