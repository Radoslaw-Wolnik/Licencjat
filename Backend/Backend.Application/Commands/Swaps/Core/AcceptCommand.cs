using Backend.Domain.Entities;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Core;

public sealed record AcceptCommand(
    Guid SwapId,
    Guid UserAcceptingId,
    Guid RequestedBookId
    ) : IRequest<Result<Swap>>;