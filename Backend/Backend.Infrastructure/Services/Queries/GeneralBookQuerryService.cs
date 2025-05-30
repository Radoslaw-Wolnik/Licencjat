using AutoMapper;
using AutoMapper.QueryableExtensions;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Enums.SortBy;
using Backend.Infrastructure.Data;
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

    public async Task<PaginatedResult<GeneralBookListItem>> ListAsync(
        string? titleFilter,
        string? authorFilter,
        BookGenre? genreFilter,
        SortGeneralBookBy sortBy,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default)
    {
        var query = _context.GeneralBooks
            .AsNoTracking()
            .Include(b => b.Reviews)
            .Include(b => b.Genres)
            .ProjectTo<GeneralBookListItem>(_mapper.ConfigurationProvider);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(titleFilter))
            query = query.Where(b => b.Title.Contains(titleFilter));
        if (!string.IsNullOrWhiteSpace(authorFilter))
            query = query.Where(b => b.Author.Contains(authorFilter));
        if (genreFilter.HasValue)
            query = query.Where(b => b.PrimaryGenre == genreFilter.Value);

        // Apply sorting
        query = sortBy switch
        {
            SortGeneralBookBy.Rating => descending
                ? query.OrderByDescending(b => b.RatingAvg)
                : query.OrderBy(b => b.RatingAvg),
            SortGeneralBookBy.Title => descending
                ? query.OrderByDescending(b => b.Title)
                : query.OrderBy(b => b.Title),
            SortGeneralBookBy.Author => descending
                ? query.OrderByDescending(b => b.Author)
                : query.OrderBy(b => b.Author),
            SortGeneralBookBy.PublicationDate => descending
                ? query.OrderByDescending(b => b.PublicationDate)
                : query.OrderBy(b => b.PublicationDate),
            _ => descending
                ? query.OrderByDescending(b => b.Title)
                : query.OrderBy(b => b.Title)
        };

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync(ct);

        return new PaginatedResult<GeneralBookListItem>(items, total);
    }

    public async Task<GeneralBookDetailsReadModel?> GetBookDetailsAsync(
        Guid bookId,
        int maxReviews = 10,
        CancellationToken ct = default)
    {
        return await _context.GeneralBooks
            .AsNoTracking()
            .Include(b => b.Genres)
            .Include(b => b.Reviews)
                .ThenInclude(r => r.User)
            .Where(b => b.Id == bookId)
            .ProjectTo<GeneralBookDetailsReadModel>(
                _mapper.ConfigurationProvider,
                parameters: new { MaxReviews = maxReviews }
            )
            .FirstOrDefaultAsync(ct);
    }

    public async Task<PaginatedResult<ReviewReadModel>> GetPaginatedReviewsAsync(
        Guid bookId,
        SortReviewsBy sortBy,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default)
    {
        var query = _context.Reviews
            .AsNoTracking()
            .Where(r => r.Book.Id == bookId)
            .Include(r => r.User)
            // .ProjectTo<ReviewReadModel>(_mapper.ConfigurationProvider);
            .ProjectTo<ReviewReadModel>(
                _mapper.ConfigurationProvider,
                parameters: new { IncludeDetails = false } // Explicitly disable details
            );

        // Apply sorting
        query = sortBy switch
        {
            SortReviewsBy.Rating => descending
                ? query.OrderByDescending(r => r.Rating)
                : query.OrderBy(r => r.Rating),
            SortReviewsBy.Date => descending
                ? query.OrderByDescending(r => r.CreatedAt)
                : query.OrderBy(r => r.CreatedAt),
            _ => descending
                ? query.OrderByDescending(r => r.CreatedAt)
                : query.OrderBy(r => r.CreatedAt)
        };

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync(ct);

        return new PaginatedResult<ReviewReadModel>(items, total);
    }
    
    public async Task<ReviewReadModel?> GetReviewByIdAsync(
        Guid reviewId,
        CancellationToken ct = default)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.Id == reviewId)
            .ProjectTo<ReviewReadModel>(
                _mapper.ConfigurationProvider,
                parameters: new { IncludeDetails = true } // Enable full details
            )
            .FirstOrDefaultAsync(ct);
    }
}