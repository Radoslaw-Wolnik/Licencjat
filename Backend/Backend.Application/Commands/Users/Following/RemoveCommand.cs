using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Following;

public sealed record RemoveFollowedUserCommand(
    Guid UserId,
    Guid UserFollowedId
    ) : IRequest<Result>;