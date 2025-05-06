using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Following;

public sealed record AddCommand(
    Guid UserId,
    Guid FollowId
    ) : IRequest<Result>;