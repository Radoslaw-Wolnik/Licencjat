using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Core;

public sealed record DeleteSwapCommand(
    Guid SwapId
    ) : IRequest<Result>;