using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Backend.Domain.Errors;

namespace Backend.Application.Commands.GeneralBooks.Reviews;

public class DeleteReviewCommandHandler
    : IRequestHandler<DeleteReviewCommand, Result>
{
    private readonly IWriteGeneralBookRepository _bookRepo;

    public DeleteReviewCommandHandler(
        IWriteGeneralBookRepository bookRepo)
    {
        _bookRepo = bookRepo;
    }

    public async Task<Result> Handle(
        DeleteReviewCommand request,
        CancellationToken cancellationToken)
    {
        return await _bookRepo.RemoveReviewAsync(request.ReviewId, cancellationToken);
    }
}
