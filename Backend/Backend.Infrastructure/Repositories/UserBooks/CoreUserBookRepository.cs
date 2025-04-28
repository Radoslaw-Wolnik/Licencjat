// Backend.Infrastructure/Repositories/UserRepository.cs
using Backend.Domain.Entities;
using Backend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using FluentResults;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Linq.Expressions;
using Backend.Domain.Common;
using AutoMapper.Extensions.ExpressionMapping;
using Backend.Domain.Errors;
using Backend.Infrastructure.Mapping;
using Backend.Application.DTOs;
using Backend.Application.DTOs.Auth;

namespace Backend.Infrastructure.Repositories.UserBooks;

// Core user books (scalars + existence)
public interface ICoreUserBookRepository
{
    Task<Result<bool>> ExistsAsync(Expression<Func<BookProjection, bool>> predicate);
    Task<Result<Guid>> AddAsync(UserBook book);
    Task<Result<UserBook>> GetByIdAsync(Guid id);
    Task<Result<UserBook>> GetByAsync(Expression<Func<BookProjection, bool>> predicate);
    Task<Result<UserBook>> GetBookWithIncludes(Guid bookId, 
        params Expression<Func<BookProjection, object>>[] includes);

    Task<Result>  UpdateAsync(UserBook book);
    Task<Result>  DeleteAsync(Guid bookId);
}



public class CoreUserBookRepository : ICoreUserBookRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CoreUserBookRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<bool>> ExistsAsync(Expression<Func<BookProjection, bool>> predicate)
    {
        var entityPredicate = 
                _mapper.MapExpression<Expression<Func<UserBookEntity, bool>>>(predicate);
        var exists = await _context.UserBooks
            .AnyAsync(entityPredicate);

        return Result.Ok(exists);
    }

    public async Task<Result<Guid>> AddAsync(UserBook book)
    {
        var dbBook = _mapper.Map<UserBookEntity>(book);
        _context.UserBooks.Add(dbBook);

        try
        {    
            await _context.SaveChangesAsync();
            return Result.Ok(dbBook.Id);
        }
        catch (DbUpdateException ex)
        {
            // Handle DB errors (e.g., unique constraint violations)
            return Result.Fail<Guid>(new DomainError(
                "DatabaseError", 
                ex.InnerException?.Message ?? ex.Message, 
                ErrorType.Conflict
        ));
        }
    }

    public async Task<Result<UserBook>> GetByIdAsync(Guid bookId)
    {
        var dbBook = await _context.UserBooks.FindAsync(bookId);
        return dbBook is null 
            ? Result.Fail<UserBook>(BookErrors.NotFound)
            : Result.Ok(_mapper.Map<UserBook>(dbBook));
    }

    public async Task<Result<UserBook>> GetByAsync(
        Expression<Func<BookProjection, bool>> predicate
    ){
        try
        {
            var entityPredicate = 
                _mapper.MapExpression<Expression<Func<UserBookEntity, bool>>>(predicate);

            var entity = await _context.UserBooks
                .FirstOrDefaultAsync(entityPredicate);
            
            if (entity is null)
                return Result.Fail<UserBook>(BookErrors.NotFound);

            // Automapper will then run your ConstructUsing(src=>MapToDomain(src))
            var book = _mapper.Map<UserBook>(entity);
            return Result.Ok(book);
        }
        catch (Exception ex)
        {
            return Result.Fail<UserBook>
                (new DomainError("DatabaseError", ex.Message, ErrorType.StorageError));
        }
    }

    public async Task<Result<UserBook>> GetBookWithIncludes(
        Guid bookId, 
        params Expression<Func<BookProjection, object>>[] includes
    ){
        try
        {
            IQueryable<UserBookEntity> query = _context.UserBooks;
            foreach (var inc in includes){
                var entityInclude = 
                    _mapper.MapExpression<Expression<Func<UserBookEntity, bool>>>(inc); // not sure if this will work
                query = query.Include(entityInclude);
            }

            var entity = await query.FirstOrDefaultAsync(b => b.Id == bookId);
            if (entity is null)
                return Result.Fail<UserBook>(BookErrors.NotFound);

            return Result.Ok(_mapper.Map<UserBook>(entity));
        }
        catch (Exception ex)
        {
            return Result.Fail<UserBook>(
                new DomainError("DatabaseError", ex.Message, ErrorType.StorageError)
            );
        }
    }

    public async Task<Result> UpdateAsync(UserBook book)
    {
        var existing = await _context.UserBooks.FindAsync(book.Id);
        if (existing == null)
            return Result.Fail(BookErrors.NotFound);

        _mapper.Map(book, existing);

        try
        {    
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // e.g. row-version mismatch
            return Result.Fail(
                new DomainError("ConcurrencyError", ex.Message, ErrorType.Conflict)
            );
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(
                new DomainError("DatabaseError", ex.InnerException?.Message ?? ex.Message, ErrorType.StorageError)
            );
        }
    }

    public async Task<Result>  DeleteAsync(Guid bookId)
    {
        var dbBook = await _context.UserBooks.FindAsync(bookId);
        if (dbBook == null)
            return Result.Fail(BookErrors.NotFound); 

        _context.UserBooks.Remove(dbBook);
        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            // mby more in future for special-cases foreign‐key / constraint errors
            return Result.Fail(
                new DomainError("DatabaseError", ex.InnerException?.Message ?? ex.Message, ErrorType.StorageError)
            );
        }
    }

}