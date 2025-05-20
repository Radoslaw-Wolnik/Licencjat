using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using System.ComponentModel;

namespace Backend.Application.Commands.GeneralBooks.Reviews;
public class CreateReviewCommandHandler
    : IRequestHandler<CreateReviewCommand, Result<Guid>>
{
    private readonly IWriteGeneralBookRepository _bookRepo;

    public CreateReviewCommandHandler(
        IWriteGeneralBookRepository bookRepo)
    {
        _bookRepo = bookRepo;
    }

    public async Task<Result<Guid>> Handle(
        CreateReviewCommand request,
        CancellationToken cancellationToken)
    {
        var reviewId = Guid.NewGuid();
        
        // create new review
        var reviewResult = Review.Create(reviewId, request.BookId, request.UserId, request.Rating, request.Comment);
        if (reviewResult.IsFailed)
            return Result.Fail(reviewResult.Errors);
        
        // saev via repository root
        var persistanceResult = await _bookRepo.AddReviewAsync(reviewResult.Value, cancellationToken);
        if(persistanceResult.IsFailed)
            return Result.Fail(persistanceResult.Errors);

        return reviewId;
    }
}
