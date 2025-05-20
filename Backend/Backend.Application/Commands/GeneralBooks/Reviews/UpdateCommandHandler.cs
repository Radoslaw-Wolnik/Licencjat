using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using System.ComponentModel;
using Backend.Application.Interfaces.DbReads;

namespace Backend.Application.Commands.GeneralBooks.Reviews;
public class UpdateReviewCommandHandler
    : IRequestHandler<UpdateReviewCommand, Result>
{
    private readonly IWriteGeneralBookRepository _bookRepo;
    private readonly IGeneralBookReadService _bookRead;

    public UpdateReviewCommandHandler(
        IWriteGeneralBookRepository bookRepo,
        IGeneralBookReadService bookReadService)
    {
        _bookRepo = bookRepo;
        _bookRead = bookReadService;
    }

    public async Task<Result> Handle(
        UpdateReviewCommand request,
        CancellationToken cancellationToken)
    {
        // load the exsisting - previous review
        var existing = await _bookRead.GetReviewByIdAsync(request.ReviewId, cancellationToken);
        if (existing == null)
            return Result.Fail("Review not found");

        var old = existing;

        // Merge in the possible differences
        var newRating  = request.Rating  ?? old.Rating;
        var newComment = request.Comment ?? old.Comment;

        var createResult = Review.Create(
            id:      old.Id,
            userId:  old.UserId,
            bookId:  old.BookId,
            rating:  newRating,
            comment: newComment
        );
        if (createResult.IsFailed)
            return Result.Fail(createResult.Errors);

        var updated = createResult.Value;

        // persistance save
        return await _bookRepo.UpdateReviewAsync(updated, cancellationToken);
    }
}
