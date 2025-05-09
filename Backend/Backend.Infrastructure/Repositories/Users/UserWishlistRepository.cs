using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;

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

    public async Task<Result> AddAsync(Guid userId, Guid bookId, CancellationToken cancellationToken)
    {
        _db.UserWishlists.Add(new UserWishlistEntity {
            UserId = userId,
            GeneralBookId = bookId
        });
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to Wishlist book");
    }

    public async Task<Result> RemoveAsync(Guid userId, Guid bookId, CancellationToken cancellationToken)
    {
        var existing = await _db.UserWishlists.FindAsync(userId, bookId);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("Wishlist", $"{userId} wishing {bookId}"));

        _db.UserWishlists.Remove(existing);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to remove Wishlisted book");
    }
}
