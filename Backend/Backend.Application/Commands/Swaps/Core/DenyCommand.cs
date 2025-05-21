using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Core;

public sealed record DenySwapCommand(
    Guid SwapId,
    Guid UserId
    ) : IRequest<Result>;