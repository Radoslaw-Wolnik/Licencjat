using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using System.Linq.Expressions;
using AutoMapper.Extensions.ExpressionMapping;
using Backend.Application.DTOs;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace Backend.Infrastructure.Services.DbReads;

public class GeneralBookReadService : IGeneralBookReadService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GeneralBookReadService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<BookProjection, bool>> predicate, 
        CancellationToken cancellationToken = default
    ) {
        var entityPredicate = 
                _mapper.MapExpression<Expression<Func<GeneralBookEntity, bool>>>(predicate);
        var exists = await _context.GeneralBooks
            .AnyAsync(entityPredicate, cancellationToken);

        return exists;
    }

    public async Task<GeneralBook> GetByIdAsync(
        Guid bookId, 
        CancellationToken cancellationToken = default
    ) {
        var dbBook = await _context.GeneralBooks.FindAsync(bookId, cancellationToken);
        //var dbBook = await _context.GeneralBooks.FindAsync([bookId, cancellationToken], cancellationToken: cancellationToken);
        return _mapper.Map<GeneralBook>(dbBook);
    }

    public async Task<GeneralBook> GetByAsync(
        Expression<Func<BookProjection, bool>> predicate,
        CancellationToken cancellationToken = default
    ) {

        var entityPredicate = 
            _mapper.MapExpression<Expression<Func<GeneralBookEntity, bool>>>(predicate);

        var entity = await _context.GeneralBooks
            .FirstOrDefaultAsync(entityPredicate, cancellationToken);

        // Automapper will then run your ConstructUsing(src=>MapToDomain(src))
        var book = _mapper.Map<GeneralBook>(entity);
        return book;

    }

    public async Task<GeneralBook> GetFullByIdAsync(
        Guid bookId,
        CancellationToken cancellationToken = default
    ) {
        var entity = await _context.GeneralBooks
            .Include(b => b.Reviews)
            .FirstAsync(b => b.Id == bookId, cancellationToken);

        return _mapper.Map<GeneralBook>(entity);
    }

    public async Task<Review> GetReviewByIdAsync(
        Guid reviewId,
        CancellationToken cancellationToken
    ) {
        var entity = await _context.Reviews.
            FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);

        return _mapper.Map<Review>(entity);
    }
}