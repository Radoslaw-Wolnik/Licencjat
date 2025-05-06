using Backend.Domain.Entities;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Core;

public sealed record UpdateCommand(
    Guid SwapId,
    Guid UserId,
    int PageAt
    ) : IRequest<Result<Swap>>; // or subswap