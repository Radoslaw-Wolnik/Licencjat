using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Application.Interfaces.DbReads;

namespace Backend.Application.Commands.Swaps.Core;
public class DeleteSwapCommandHandler
    : IRequestHandler<DeleteSwapCommand, Result>
{
    private readonly IWriteSwapRepository _swapRepo;

    public DeleteSwapCommandHandler(
        IWriteSwapRepository swapRepository)
    {
        _swapRepo = swapRepository;
    }

    public async Task<Result> Handle(
        DeleteSwapCommand request,
        CancellationToken cancellationToken)
    {
        // persist changes
        // var persistanceResult =
        return await _swapRepo.DeleteAsync(request.SwapId, cancellationToken);
    }
}
