using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using AutoMapper.QueryableExtensions;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Enums.SortBy;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Services.Queries;

public class GeneralBookQueryService : IGeneralBookQueryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GeneralBookQueryService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    } 
    
    public async Task<bool> ExistsAsync(
        Expression<Func<GeneralBookProjection, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        var entityPredicate =
                _mapper.MapExpression<Expression<Func<GeneralBookEntity, bool>>>(predicate);
        var exists = await _context.GeneralBooks
            .AnyAsync(entityPredicate, cancellationToken);

        return exists;
    }
    
    
     public async Task<PaginatedResult<GeneralBookListItem>> ListAsync(
        string? titleFilter,
        string? authorFilter,
        BookGenre? genreFilter,
        SortGeneralBookBy sortBy,
        bool descending,
        int Offset,
        int Limit,
        CancellationToken ct = default
    )
    {
        // 1) start with projection to DTO (so you only fetch needed columns)
        var query = _context.GeneralBooks
            .AsNoTracking()
            .ProjectTo<GeneralBookProjection>(_mapper.ConfigurationProvider)
            .AsQueryable();

        // var entityPredicate =
        //     _mapper.MapExpression<Expression<Func<GeneralBookEntity, bool>>>(predicate);

        // 2) apply optional filters (use Contains for “fuzzy” search)
        if (!string.IsNullOrWhiteSpace(titleFilter))
            query = query.Where(b => b.Title.Contains(titleFilter));
        if (!string.IsNullOrWhiteSpace(authorFilter))
            query = query.Where(b => b.Author.Contains(authorFilter));
        if (genreFilter.HasValue)
            query = query.Where(b => b.BookGenre == genreFilter.Value);

        // 3) get total count _before_ paging
        var totalCount = await query.CountAsync(ct);

        // 4) apply sorting
        query = (sortBy, descending) switch
        {
            (SortGeneralBookBy.Rating, true) => query.OrderByDescending(b => b.RatingAvg),
            (SortGeneralBookBy.Rating, false) => query.OrderBy(b => b.RatingAvg),
            (SortGeneralBookBy.Title, true) => query.OrderByDescending(b => b.Title),
            (SortGeneralBookBy.Title, false) => query.OrderBy(b => b.Title),
            (SortGeneralBookBy.Author, true) => query.OrderByDescending(b => b.Author),
            (SortGeneralBookBy.Author, false) => query.OrderBy(b => b.Author),
            (SortGeneralBookBy.PublicationDate, true) => query.OrderByDescending(b => b.PublicationDate),
            (SortGeneralBookBy.PublicationDate, false) => query.OrderBy(b => b.PublicationDate),
            _ => query.OrderBy(b => b.Title)
        };

        // 5) apply pagination
        var items = await query
            .Skip(Offset)
            .Take(Limit)
            .ToListAsync(ct);

        // 6) map projections into your domain/DTO if needed
        var domainItems = items.Select(_mapper.Map<GeneralBookListItem>).ToList();

        return new PaginatedResult<GeneralBookListItem>(domainItems, totalCount);
    }

    public async Task<GeneralBookDetailsReadModel?> GetBookDetailsAsync(
        Guid bookId,
        int maxReviews = 10,
        CancellationToken ct = default
    )
    {
        var model = await _context.GeneralBooks
            .AsNoTracking()
            .Where(b => b.Id == bookId)
            .Select(b => new GeneralBookDetailsReadModel(
                b.Id,
                b.Title,
                b.Author,
                b.Published,
                b.Language,
                b.Reviews.Count != 0 ? b.Reviews.Average(r => r.Rating) : 0f,
                b.CoverPhoto,
                b.Genres,
                b.Reviews
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(maxReviews)
                    .Select(r => new ReviewReadModel(
                        r.Id,
                        new UserSmallReadModel(
                            r.User.Id,
                            r.User.UserName??"__no__username__error__",
                            r.User.ProfilePicture
                        ),
                        r.Rating,
                        r.Comment
                    ))
                    .ToList()
            ))
            .FirstOrDefaultAsync(ct);

        return model;
    }
}
