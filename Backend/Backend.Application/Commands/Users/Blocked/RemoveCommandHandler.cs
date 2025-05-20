using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Blocked;

public class RemoveBlockedUserCommandHandler
    : IRequestHandler<RemoveBlockedUserCommand, Result>
{
    private readonly IWriteUserRepository _userRepo;
    public RemoveBlockedUserCommandHandler(
        IWriteUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<Result> Handle(
        RemoveBlockedUserCommand request,
        CancellationToken cancellationToken)
    {
        return await _userRepo.RemoveBlockedUserAsync(request.UserId, request.UserBlockedId, cancellationToken);
    }
}
