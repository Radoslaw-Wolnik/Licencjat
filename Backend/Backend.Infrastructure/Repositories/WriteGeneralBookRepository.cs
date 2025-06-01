using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using Backend.Infrastructure.Extensions;
using FluentResults;
using Backend.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class WriteGeneralBookRepository : IWriteGeneralBookRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public WriteGeneralBookRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> AddAsync(GeneralBook book, CancellationToken cancellationToken)
    {
        var dbBook = _mapper.Map<GeneralBookEntity>(book);
        _context.GeneralBooks.Add(dbBook);

        // await _context.SaveChangesAsync(cancellationToken);
        var result = await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to add book");
        
        return result.IsSuccess
            ? Result.Ok(dbBook.Id)
            : Result.Fail<Guid>(result.Errors);
    }

    public async Task<Result> DeleteAsync(Guid bookId, CancellationToken cancellationToken)
    {
        // var existing = await _context.GeneralBooks.FindAsync(bookId, cancellationToken);
        var existing = await _context.GeneralBooks.FindAsync([bookId], cancellationToken);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("GeneralBook", bookId));
            
        _context.GeneralBooks.Remove(existing);

        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to delete book");
    }

    // 1) Scalar-only update (no touching genres, copies, reviews)
    public async Task<Result> UpdateScalarsAsync(GeneralBook book, CancellationToken cancellationToken)
    {
        var stub = new GeneralBookEntity { Id = book.Id };
        _context.GeneralBooks.Attach(stub);

        stub.Title = book.Title;
        stub.Author = book.Author;
        stub.Published = book.Published;
        stub.Language = book.OriginalLanguage.Code;
        stub.CoverPhoto = book.CoverPhoto.Link;
        //  RatingAvg not stored as filed in db

        var entry = _context.Entry(stub);
        entry.Property(e => e.Title).IsModified = true;
        entry.Property(e => e.Author).IsModified = true;
        entry.Property(e => e.Published).IsModified = true;
        entry.Property(e => e.Language).IsModified = true;
        entry.Property(e => e.CoverPhoto).IsModified  = true;

        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to update the book");
    }

    // Add a single review without loading all reviews
    public async Task<Result> AddReviewAsync(Guid generalBookId, Review review, CancellationToken cancellationToken)
    {
        if (!await _context.GeneralBooks.AnyAsync(g => g.Id == generalBookId, cancellationToken))
            return Result.Fail(DomainErrorFactory.NotFound("GeneralBook", generalBookId));

        var revEntity = _mapper.Map<ReviewEntity>(review);
        revEntity.BookId = generalBookId;

        _context.Reviews.Add(revEntity);

        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to add a new review");
    }

    // Update a specific review (without loading the full collection - all reviews)
    public async Task<Result> UpdateReviewAsync(Review review, CancellationToken cancellationToken)
    {
        var exists = await _context.Reviews.AnyAsync(r => r.Id == review.Id, cancellationToken);
            if (!exists)
                return Result.Fail(DomainErrorFactory.NotFound("Review", review.Id));

        // attach stub for review
        var revEntity = new ReviewEntity { Id = review.Id };
        _context.Reviews.Attach(revEntity);
        revEntity.Rating  = review.Rating;
        revEntity.Comment = review.Comment;

        var entry = _context.Entry(revEntity);
        entry.Property(e => e.Rating).IsModified  = true;
        entry.Property(e => e.Comment).IsModified = true;

        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to update reviews");
    }

    // Delete a specific review (without loading the full collection - all reviews)
    public async Task<Result> RemoveReviewAsync(Guid reviewId, CancellationToken cancellationToken)
    {
        var rev = await _context.Reviews.FindAsync([reviewId], cancellationToken);
        if (rev == null)
            return Result.Fail(DomainErrorFactory.NotFound("Review", reviewId));

        _context.Reviews.Remove(rev);

        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to remove the review");
    }
}
