using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Backend.Domain.Errors;

namespace Backend.Application.Commands.Swaps.Issues;

public class RemoveIssueCommandHandler
    : IRequestHandler<RemoveIssueCommand, Result>
{
    private readonly IWriteSwapRepository _swapRepo;

    public RemoveIssueCommandHandler(
        IWriteSwapRepository swapRepo)
    {
        _swapRepo = swapRepo;
    }

    public async Task<Result> Handle(
        RemoveIssueCommand request,
        CancellationToken cancellationToken)
    {
        return await _swapRepo.RemoveIssueAsync(request.IssueId, cancellationToken);
    }
}
