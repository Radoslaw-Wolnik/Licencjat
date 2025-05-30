using AutoMapper;
using AutoMapper.QueryableExtensions;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.UserBooks;
using Backend.Domain.Common;
using Backend.Domain.Enums.SortBy;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Services.Queries
{
    public class UserBookQueryService : IUserBookQueryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserBookQueryService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<UserBookListItem>> ListAsync(
            Guid generalBookId,
            SortUserBookBy sortBy,
            bool descending,
            int offset,
            int limit,
            CancellationToken ct = default)
        {
            var baseQuery = _context.UserBooks
                .AsNoTracking()
                .Where(ub => ub.BookId == generalBookId);

            // Total count before sorting/pagination
            var totalCount = await baseQuery.CountAsync(ct);

            // Project and sort
            var query = baseQuery.ProjectTo<UserBookProjection>(_mapper.ConfigurationProvider);
            query = ApplySorting(query, sortBy, descending);

            var projections = await query
                .Skip(offset)
                .Take(limit)
                .ToListAsync(ct);

            var items = _mapper.Map<List<UserBookListItem>>(projections);
            return new PaginatedResult<UserBookListItem>(items, totalCount);
        }

        public async Task<PaginatedResult<UserLibraryListItem>> ListLibraryAsync(
            Guid userId,
            string? titleFilter,
            string? authorFilter,
            SortUserBookBy sortBy,
            bool descending,
            int offset,
            int limit,
            CancellationToken ct = default)
        {
            var query = _context.UserBooks
                .AsNoTracking()
                .Where(ub => ub.UserId == userId)
                .ProjectTo<UserLibraryListItem>(_mapper.ConfigurationProvider);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(titleFilter))
                query = query.Where(x => x.Title.Contains(titleFilter));
            if (!string.IsNullOrWhiteSpace(authorFilter))
                query = query.Where(x => x.Author.Contains(authorFilter));

            var totalCount = await query.CountAsync(ct);
            query = ApplyLibrarySorting(query, sortBy, descending);

            var items = await query
                .Skip(offset)
                .Take(limit)
                .ToListAsync(ct);

            return new PaginatedResult<UserLibraryListItem>(items, totalCount);
        }

        public async Task<UserBookDetailsReadModel?> GetBookDetailsAsync(
            Guid bookId, CancellationToken ct = default)
        {
            return await _context.UserBooks
                .AsNoTracking()
                .Where(ub => ub.Id == bookId)
                .ProjectTo<UserBookDetailsReadModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<UserOwnBookProfileReadModel?> GetOwnBookDetailsAsync(
            Guid bookId, int maxBookmarks = 10, CancellationToken ct = default)
        {
            return await _context.UserBooks
                .AsNoTracking()
                .Where(ub => ub.Id == bookId)
                .ProjectTo<UserOwnBookProfileReadModel>(_mapper.ConfigurationProvider, 
                    new Dictionary<string, object> { { "maxBookmarks", maxBookmarks } })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<PaginatedResult<BookmarkReadModel>> ListBookmarksAsync(
            Guid bookId, bool descending, int offset, int limit, CancellationToken ct = default)
        {
            var query = _context.Bookmarks
                .AsNoTracking()
                .Where(b => b.UserBookId == bookId)
                .ProjectTo<BookmarkReadModel>(_mapper.ConfigurationProvider);

            query = descending 
                ? query.OrderByDescending(b => b.Page) 
                : query.OrderBy(b => b.Page);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .Skip(offset)
                .Take(limit)
                .ToListAsync(ct);

            return new PaginatedResult<BookmarkReadModel>(items, totalCount);
        }
        
        public async Task<BookmarkReadModel?> GetBookmarkByIdAsync(
            Guid bookmarkId,
            CancellationToken ct = default)
        {
            return await _context.Bookmarks
                .AsNoTracking()
                .Where(b => b.Id == bookmarkId)
                .ProjectTo<BookmarkReadModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);
        }

        private IQueryable<UserBookProjection> ApplySorting(
            IQueryable<UserBookProjection> query,
            SortUserBookBy sortBy,
            bool descending)
        {
            return (sortBy, descending) switch
            {
                (SortUserBookBy.Title, true) => query.OrderByDescending(x => x.Title),
                (SortUserBookBy.Title, false) => query.OrderBy(x => x.Title),
                (SortUserBookBy.Author, true) => query.OrderByDescending(x => x.Author),
                (SortUserBookBy.Author, false) => query.OrderBy(x => x.Author),
                (SortUserBookBy.UserReputation, true) => query.OrderByDescending(x => x.UserReputation),
                (SortUserBookBy.UserReputation, false) => query.OrderBy(x => x.UserReputation),
                _ => query.OrderBy(x => x.Id)
            };
        }

        private IQueryable<UserLibraryListItem> ApplyLibrarySorting(
            IQueryable<UserLibraryListItem> query, 
            SortUserBookBy sortBy, 
            bool descending)
        {
            return (sortBy, descending) switch
            {
                (SortUserBookBy.Title, true) => query.OrderByDescending(x => x.Title),
                (SortUserBookBy.Title, false) => query.OrderBy(x => x.Title),
                (SortUserBookBy.Author, true) => query.OrderByDescending(x => x.Author),
                (SortUserBookBy.Author, false) => query.OrderBy(x => x.Author),
                (SortUserBookBy.BookAvgRating, true) => query.OrderByDescending(x => x.RatingAvg),
                (SortUserBookBy.BookAvgRating, false) => query.OrderBy(x => x.RatingAvg),
                _ => query.OrderBy(x => x.Title)
            };
        }
    }
}