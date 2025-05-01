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

public interface IUserBookReadService
{
    Task<UserBook> GetByIdAsync(Guid id);
    Task<UserBook> GetByAsync(Expression<Func<BookProjection, bool>> predicate);
    Task<UserBook> GetBookWithIncludes(Guid bookId, 
        params Expression<Func<BookProjection, object>>[] includes);
}



public class UserBookReadService : IUserBookReadService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserBookReadService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserBook> GetByIdAsync(Guid bookId)
    {
        var dbBook = await _context.UserBooks.FindAsync(bookId);
        return _mapper.Map<UserBook>(dbBook);
    }

    public async Task<UserBook> GetByAsync(
        Expression<Func<BookProjection, bool>> predicate
    ){
        var entityPredicate = 
            _mapper.MapExpression<Expression<Func<UserBookEntity, bool>>>(predicate);

        var entity = await _context.UserBooks
            .FirstOrDefaultAsync(entityPredicate);
        
        return _mapper.Map<UserBook>(entity);
    }

    public async Task<UserBook> GetBookWithIncludes(
        Guid bookId, 
        params Expression<Func<BookProjection, object>>[] includes
    ){
        IQueryable<UserBookEntity> query = _context.UserBooks;
        foreach (var inc in includes){
            var entityInclude = 
                _mapper.MapExpression<Expression<Func<UserBookEntity, bool>>>(inc); // not sure if this will work
            query = query.Include(entityInclude);
        }

        var entity = await query.FirstOrDefaultAsync(b => b.Id == bookId);

        return _mapper.Map<UserBook>(entity);
    }
}