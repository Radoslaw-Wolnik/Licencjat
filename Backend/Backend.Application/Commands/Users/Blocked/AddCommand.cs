using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Blocked;

public sealed record AddCommand(
    Guid UserId,
    Guid BlockId
    ) : IRequest<Result>;