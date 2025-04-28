using AutoMapper;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories.Users;

// Wishlist (ID‑only nav)
public interface IUserWishlistRepository
{
    Task<Result<IReadOnlyCollection<Guid>>> GetWishlistAsync(Guid userId);

    Task<Result> AddToWishlistAsync(Guid userId, Guid generalBookId);
    Task<Result> RemoveFromWishlistAsync(Guid userId, Guid generalBookId);
    // Task<Result> UpdateAsync(Guid userId, Guid generalBookId); it doesnt really make sense to make an update for this agregate
    
    Task<Result<bool>> WishlistContainsAsync(Guid userId, Guid generalBookId);
    
}

public class UserWishlistRepository : IUserWishlistRepository
{
    private readonly ApplicationDbContext _context;

    public UserWishlistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyCollection<Guid>>> GetWishlistAsync(Guid userId)
    {
        var list = await _context.GeneralBooks
            .Where(gb => gb.WishlistedByUsers.Any(u => u.Id == userId))
            .Select(gb => gb.Id)
            .ToListAsync();

        return Result.Ok((IReadOnlyCollection<Guid>)list);
    }

    public async Task<Result> AddToWishlistAsync(Guid userId, Guid bookId)
    {
        var user = await _context.Users
            .Include(u => u.Wishlist)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) 
            return Result.Fail(UserErrors.NotFound);

        // check duplicate
        if (user.Wishlist.Any(gb => gb.Id == bookId))
            return Result.Fail(WishlistErrors.ItemExists);

        // load the book
        var book = await _context.GeneralBooks
            .FirstOrDefaultAsync(gb => gb.Id == bookId);
        if (book is null) 
            return Result.Fail(BookErrors.NotFound);

        // add and save
        user.Wishlist.Add(book);
        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new DomainError(
                "DatabaseError", ex.Message, ErrorType.StorageError));
        }
    }


    public async Task<Result> RemoveFromWishlistAsync(Guid userId, Guid bookId)
    {
        var user = await _context.Users
            .Include(u => u.Wishlist)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return Result.Fail(UserErrors.NotFound);

        // find the exact book in their wishlist
        var book = user.Wishlist.FirstOrDefault(gb => gb.Id == bookId);
        if (book is null)
            return Result.Fail(WishlistErrors.NotFound);

        user.Wishlist.Remove(book);
        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new DomainError(
                "DatabaseError", ex.Message, ErrorType.StorageError));
        }
    }


    public async Task<Result<bool>> WishlistContainsAsync(Guid userId, Guid bookId)
    {
        var exists = await _context.GeneralBooks
            .AnyAsync(gb =>
                gb.Id == bookId &&
                gb.WishlistedByUsers.Any(u => u.Id == userId));
        return Result.Ok(exists);
    }

}