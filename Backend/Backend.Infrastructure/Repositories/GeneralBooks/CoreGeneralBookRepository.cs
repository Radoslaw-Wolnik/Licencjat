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

namespace Backend.Infrastructure.Repositories.GeneralBooks;

// Core general books (scalars + existence)
public interface ICoreGeneralBookRepository
{
    Task<Result<bool>> ExistsAsync(Expression<Func<BookProjection, bool>> predicate);
    Task<Result<Guid>> AddAsync(GeneralBook user);
    Task<Result<GeneralBook>> GetByIdAsync(Guid id);
    Task<Result<GeneralBook>> GetByAsync(Expression<Func<BookProjection, bool>> predicate);
    Task<Result<GeneralBook>> GetBookWithIncludes(Guid bookId, 
        params Expression<Func<BookProjection, object>>[] includes);

    Task<Result>  UpdateAsync(GeneralBook book);
    Task<Result>  DeleteAsync(Guid bookId);
}



public class CoreGeneralBookRepository : ICoreGeneralBookRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CoreGeneralBookRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<bool>> ExistsAsync(Expression<Func<BookProjection, bool>> predicate)
    {
        var entityPredicate = 
                _mapper.MapExpression<Expression<Func<GeneralBookEntity, bool>>>(predicate);
        var exists = await _context.GeneralBooks
            .AnyAsync(entityPredicate);

        return Result.Ok(exists);
    }

    public async Task<Result<Guid>> AddAsync(GeneralBook book)
    {
        var dbBook = _mapper.Map<GeneralBookEntity>(book);
        _context.GeneralBooks.Add(dbBook);

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

    public async Task<Result<GeneralBook>> GetByIdAsync(Guid bookId)
    {
        var dbBook = await _context.GeneralBooks.FindAsync(bookId);
        return dbBook is null 
            ? Result.Fail<GeneralBook>(BookErrors.NotFound)
            : Result.Ok(_mapper.Map<GeneralBook>(dbBook));
    }

    public async Task<Result<GeneralBook>> GetByAsync(
        Expression<Func<BookProjection, bool>> predicate
    ){
        try
        {
            var entityPredicate = 
                _mapper.MapExpression<Expression<Func<GeneralBookEntity, bool>>>(predicate);

            var entity = await _context.GeneralBooks
                .FirstOrDefaultAsync(entityPredicate);
            
            if (entity is null)
                return Result.Fail<GeneralBook>(BookErrors.NotFound);

            // Automapper will then run your ConstructUsing(src=>MapToDomain(src))
            var book = _mapper.Map<GeneralBook>(entity);
            return Result.Ok(book);
        }
        catch (Exception ex)
        {
            return Result.Fail<GeneralBook>
                (new DomainError("DatabaseError", ex.Message, ErrorType.StorageError));
        }
    }

    public async Task<Result<GeneralBook>> GetBookWithIncludes(
        Guid bookId, 
        params Expression<Func<BookProjection, object>>[] includes
    ){
        try
        {
            IQueryable<GeneralBookEntity> query = _context.GeneralBooks;
            foreach (var inc in includes){
                var entityInclude = 
                    _mapper.MapExpression<Expression<Func<GeneralBookEntity, bool>>>(inc); // not sure if this will work
                query = query.Include(entityInclude);
            }

            var entity = await query.FirstOrDefaultAsync(b => b.Id == bookId);
            if (entity is null)
                return Result.Fail<GeneralBook>(BookErrors.NotFound);

            return Result.Ok(_mapper.Map<GeneralBook>(entity));
        }
        catch (Exception ex)
        {
            return Result.Fail<GeneralBook>(
                new DomainError("DatabaseError", ex.Message, ErrorType.StorageError)
            );
        }
    }

    public async Task<Result> UpdateAsync(GeneralBook book)
    {
        var existing = await _context.GeneralBooks.FindAsync(book.Id);
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
        var dbBook = await _context.GeneralBooks.FindAsync(bookId);
        if (dbBook == null)
            return Result.Fail(BookErrors.NotFound); 

        _context.GeneralBooks.Remove(dbBook);
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