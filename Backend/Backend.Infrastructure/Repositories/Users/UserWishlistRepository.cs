using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;

namespace Backend.Infrastructure.Repositories.Users;

public class UserWishlistRepository : IUserWishlistRepository
{
    private readonly ApplicationDbContext _db;

    public UserWishlistRepository(ApplicationDbContext db)
        => _db = db;

    public async Task<IReadOnlyCollection<Guid>> GetByUserIdAsync(Guid userId)
    {   
        return await _db.UserWishlists
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.GeneralBookId)
            .ToListAsync();
    }

    public async Task AddAsync(Guid userId, Guid bookId)
    {
        _db.UserWishlists.Add(new UserWishlistEntity {
            UserId = userId,
            GeneralBookId = bookId
        });
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid userId, Guid bookId)
    {
        var existing = await _db.UserWishlists.FindAsync(userId, bookId);
        if (existing is null)
            throw new KeyNotFoundException($"Wishlist of user {userId} of generalbook {bookId} was not found.");

        _db.UserWishlists.Remove(existing);
    }
}
