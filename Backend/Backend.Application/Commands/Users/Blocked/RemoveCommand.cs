using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Blocked;

public sealed record RemoveBlockedUserCommand(
    Guid UserId,
    Guid UserBlockedId
    ) : IRequest<Result>;