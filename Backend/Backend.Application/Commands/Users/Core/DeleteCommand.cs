using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Core;

public sealed record DeleteUserCommand(
    Guid UserId
    ) : IRequest<Result>;