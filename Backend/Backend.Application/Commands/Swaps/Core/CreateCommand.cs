using Backend.Domain.Entities;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Core;

public sealed record CreateCommand(
    Guid UserRequestingId,
    Guid UserAcceptingId,
    Guid RequestedBookId
    ) : IRequest<Result<Swap>>;