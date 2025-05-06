using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Core;

public sealed record DeleteCommand(
    Guid SwapId
    ) : IRequest<Result>;