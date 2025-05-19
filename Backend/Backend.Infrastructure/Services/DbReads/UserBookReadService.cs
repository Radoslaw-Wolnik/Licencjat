using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using System.Linq.Expressions;
using AutoMapper.Extensions.ExpressionMapping;
using Backend.Application.DTOs;
using Backend.Application.Interfaces.DbReads;

namespace Backend.Infrastructure.Services.DbReads;

public class UserBookReadService : IUserBookReadService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserBookReadService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserBook> GetByIdAsync(
        Guid bookId, 
        CancellationToken cancellationToken = default
    ) {
        var dbBook = await _context.UserBooks.FindAsync(bookId, cancellationToken);
        return _mapper.Map<UserBook>(dbBook);
    }

    public async Task<UserBook> GetByAsync(
        Expression<Func<BookProjection, bool>> predicate,
        CancellationToken cancellationToken = default
    ) {
        var entityPredicate = 
            _mapper.MapExpression<Expression<Func<UserBookEntity, bool>>>(predicate);

        var entity = await _context.UserBooks
            .FirstOrDefaultAsync(entityPredicate, cancellationToken);
        
        return _mapper.Map<UserBook>(entity);
    }

    public async Task<UserBook> GetFullByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) {
        var entity = await _context.UserBooks
            .Include(ub => ub.Bookmarks)
            .FirstAsync(ub => ub.Id == id, cancellationToken);

        return _mapper.Map<UserBook>(entity);
    }
}