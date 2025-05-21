using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Backend.Domain.Errors;

namespace Backend.Application.Commands.Swaps.Meetups;

public class RemoveMeetupCommandHandler
    : IRequestHandler<RemoveMeetupCommand, Result>
{
    private readonly IWriteSwapRepository _swapRepo;

    public RemoveMeetupCommandHandler(
        IWriteSwapRepository swapRepo)
    {
        _swapRepo = swapRepo;
    }

    public async Task<Result> Handle(
        RemoveMeetupCommand request,
        CancellationToken cancellationToken)
    {
        // add timeline update
        return await _swapRepo.RemoveMeetupAsync(request.MeetupId, cancellationToken);
    }
}
