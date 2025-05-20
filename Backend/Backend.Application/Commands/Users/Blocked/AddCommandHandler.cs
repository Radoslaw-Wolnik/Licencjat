using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Blocked;

public class AddBlockedUserCommandHandler
    : IRequestHandler<AddBlockedUserCommand, Result>
{
    private readonly IWriteUserRepository _userRepo;
    public AddBlockedUserCommandHandler(
        IWriteUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<Result> Handle(
        AddBlockedUserCommand request,
        CancellationToken cancellationToken)
    {
        return await _userRepo.AddBlockedUserAsync(request.UserId, request.UserBlockedId, cancellationToken);
    }
}
