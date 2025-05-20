using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Following;

public class RemoveFollowedUserCommandHandler
    : IRequestHandler<RemoveFollowedUserCommand, Result>
{
    private readonly IWriteUserRepository _userRepo;
    public RemoveFollowedUserCommandHandler(
        IWriteUserRepository userRepo
    ) {
        _userRepo = userRepo;
    }

    public async Task<Result> Handle(
        RemoveFollowedUserCommand request,
        CancellationToken cancellationToken
    ) {
        return await _userRepo.RemoveFollowingUserAsync(request.UserId, request.UserFollowedId, cancellationToken);
    }
}
