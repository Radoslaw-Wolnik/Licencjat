using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Following;

public class AddFollowedUserCommandHandler
    : IRequestHandler<AddFollowedUserCommand, Result>
{
    private readonly IWriteUserRepository _userRepo;
    public AddFollowedUserCommandHandler(
        IWriteUserRepository userRepo
    ) {
        _userRepo = userRepo;
    }

    public async Task<Result> Handle(
        AddFollowedUserCommand request,
        CancellationToken cancellationToken
    ) {
        return await _userRepo.AddFollowingUserAsync(request.UserId, request.UserFollowedId, cancellationToken);
    }
}
