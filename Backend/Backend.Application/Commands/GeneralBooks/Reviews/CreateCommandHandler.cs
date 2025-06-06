using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Backend.Domain.Common;


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
        var reviewResult = Review.Create(reviewId, request.BookId, request.UserId, request.Rating, DateTime.Now, request.Comment);
        if (reviewResult.IsFailed)
            return Result.Fail(reviewResult.Errors);
        
        // saev via repository root
        var persistanceResult = await _bookRepo.AddReviewAsync(request.BookId, reviewResult.Value, cancellationToken);
        if(persistanceResult.IsFailed)
            return Result.Fail(persistanceResult.Errors);

        return Result.Ok(reviewId);
    }
}
