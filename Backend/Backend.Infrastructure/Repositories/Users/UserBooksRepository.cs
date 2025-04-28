using AutoMapper;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories.Users;

// User‑book (rich nav)
// change to use automapper <--------------- <---
public interface IUserBooksRepository
{
    Task<Result<IReadOnlyCollection<UserBook>>> GetUserBooksAsync(Guid userId);

    Task<Result> AddUserBookAsync(Guid userId, UserBook userBook);
    Task<Result> RemoveUserBookAsync(Guid userId, Guid bookId);
    Task<Result> UpdateUserBookAsync(Guid userId, UserBook userBook);

    Task<Result<bool>> UserBookContainsAsync(Guid userId, Guid userBookId);
    
}

public class UserBooksRepository : IUserBooksRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserBooksRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<UserBook>>> GetUserBooksAsync(Guid userId)
    {
        var data = await _context.UserBooks
            .Where(s => s.UserId == userId)
            .ToListAsync();
        
        var mappedList = _mapper.Map<IReadOnlyCollection<UserBook>>(data);

        return Result.Ok(mappedList);
    }

    public async Task<Result> AddUserBookAsync(Guid userId, UserBook book)
    {
        var user = await _context.Users
            .Include(u => u.UserBooks)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return Result.Fail(UserErrors.NotFound);
        
        // check for duplicate
        if (user.UserBooks.Any(ub => ub.Id == book.Id))
            return Result.Fail(UserBookErrors.UserBookExists);
        
        // add
        var entity = _mapper.Map<UserBookEntity>(book);
        entity.UserId = userId;   // make sure the FK is set
        user.UserBooks.Add(entity);

        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(
                new DomainError("DatabaseError", ex.Message, ErrorType.StorageError)
            );
        }
    }

    public async Task<Result> RemoveUserBookAsync(Guid userId, Guid bookId)
    {
        var entity = await _context.UserBooks
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == bookId);
        if (entity is null) return Result.Fail(UserErrors.NotFound);

        _context.UserBooks.Remove(entity);

        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(
                new DomainError("DatabaseError", ex.Message, ErrorType.StorageError)
            );
        }
    }

    
    public async Task<Result> UpdateUserBookAsync(Guid userId, UserBook updated){
        var user = await _context.Users
            .Include(u => u.UserBooks)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user is null) return Result.Fail(UserErrors.NotFound);
        
        var entity = user.UserBooks
            .SingleOrDefault(ub => ub.Id == updated.Id);

        if (entity is null)
            return Result.Fail(UserBookErrors.NotFound);
        
        _mapper.Map(updated, entity);

        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(
                new DomainError("DatabaseError", ex.Message, ErrorType.StorageError)
            );
        }
    }

    public async Task<Result<bool>> UserBookContainsAsync(Guid userId, Guid userBookId)
    {
        var exists = await _context.UserBooks
            .AnyAsync(s => s.UserId == userId && s.Id == userBookId);

        return Result.Ok(exists);
    }

}