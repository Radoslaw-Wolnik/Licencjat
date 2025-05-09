using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Core;

public sealed record DenyCommand(
    Guid SwapId,
    Guid UserId
    ) : IRequest<Result>;