using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using System.Linq.Expressions;
using AutoMapper.Extensions.ExpressionMapping;
using Backend.Application.DTOs;
using Backend.Domain.Errors;
using Backend.Application.Interfaces;

namespace Backend.Infrastructure.Services.DbReads;

public interface IGeneralBookReadService
{
    Task<bool> ExistsAsync(Expression<Func<BookProjection, bool>> predicate);
    Task<GeneralBook> GetByIdAsync(Guid id);
    Task<GeneralBook> GetByAsync(Expression<Func<BookProjection, bool>> predicate);
    Task<GeneralBook> GetBookWithIncludes(Guid bookId, 
        params Expression<Func<BookProjection, object>>[] includes);
}



public class GeneralBookReadService : IGeneralBookReadService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GeneralBookReadService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> ExistsAsync(Expression<Func<BookProjection, bool>> predicate)
    {
        var entityPredicate = 
                _mapper.MapExpression<Expression<Func<GeneralBookEntity, bool>>>(predicate);
        var exists = await _context.GeneralBooks
            .AnyAsync(entityPredicate);

        return exists;
    }

    public async Task<GeneralBook> GetByIdAsync(Guid bookId)
    {
        var dbBook = await _context.GeneralBooks.FindAsync(bookId);
        return _mapper.Map<GeneralBook>(dbBook);
    }

    public async Task<GeneralBook> GetByAsync(
        Expression<Func<BookProjection, bool>> predicate
    ){

        var entityPredicate = 
            _mapper.MapExpression<Expression<Func<GeneralBookEntity, bool>>>(predicate);

        var entity = await _context.GeneralBooks
            .FirstOrDefaultAsync(entityPredicate);

        // Automapper will then run your ConstructUsing(src=>MapToDomain(src))
        var book = _mapper.Map<GeneralBook>(entity);
        return book;

    }

    public async Task<GeneralBook> GetBookWithIncludes(
        Guid bookId, 
        params Expression<Func<BookProjection, object>>[] includes
    ){

        IQueryable<GeneralBookEntity> query = _context.GeneralBooks;
        foreach (var inc in includes){
            var entityInclude = 
                _mapper.MapExpression<Expression<Func<GeneralBookEntity, bool>>>(inc); // not sure if this will work
            query = query.Include(entityInclude);
        }

        var entity = await query.FirstOrDefaultAsync(b => b.Id == bookId);

        return _mapper.Map<GeneralBook>(entity);
    }

}